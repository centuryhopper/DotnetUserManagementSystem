// javascript key concept:
// var ignores blocked scopes and has hoisting
// let is block scoped and has no hoisting
// const is block scoped and has no hoisting
// var is global, let is local

function getActiveElementClass() {
  return document.activeElement?.className || ""
}

function resetBeforeUnloads() {
  window.removeEventListener("beforeunload", handleBeforeUnload);
}

function handleBeforeUnload(event) {
  event.preventDefault();
  event.returnValue = "";
}

// IMPORTANT: Developers should only be calling methods within these objects below from blazor

FORM_HANDLING = {
  getForms: () => {
    let forms = document.querySelectorAll("form");
    //console.log(forms);
    const trackFormDirtiness = (form) => {
      let isDirty = false;
      const initialValues = {};

      // Store initial values
      Array.from(form.elements).forEach((element) => {
        if (element.name) {
          initialValues[element.name] = element.value;
        }
      });

      // Input change handler
      function checkDirty() {
        isDirty = Array.from(form.elements).some((element) => {
          return element.value !== initialValues[element.name];
        });

        // console.log('isDirty: '+isDirty);

        if (isDirty) {
          window.addEventListener("beforeunload", handleBeforeUnload);
        } else {
          window.removeEventListener("beforeunload", handleBeforeUnload);
        }
      }

      form.addEventListener("input", () => {
        checkDirty();
      });
      form.addEventListener("submit", () => {
        isDirty = false;
        resetBeforeUnloads();
      });
    };

    forms.forEach((f) => {
      trackFormDirtiness(f);
    });
  },
};

SESSION_FUNCTIONS = {
  run: (basePath, idleTimeOut, id, JWT_TOKEN_NAME, JWT_TOKEN_EXP_DATE_NAME) => {
    let jwtTimeout = undefined;
    let idleTimer = undefined;
    let countDownInterval = undefined;
    let startIdleTimerTimeout = undefined;
    const resetAllTimers = () => {
      clearInterval(countDownInterval);
      clearTimeout(jwtTimeout);
      clearTimeout(idleTimer);
      clearTimeout(startIdleTimerTimeout);
    };

    const showSessionExpiredPopup = (msg) => {
      localStorage.removeItem(JWT_TOKEN_NAME);
      localStorage.removeItem(JWT_TOKEN_EXP_DATE_NAME);
      sessionStorage.removeItem(JWT_TOKEN_NAME);
      sessionStorage.removeItem(JWT_TOKEN_EXP_DATE_NAME);
      resetBeforeUnloads();
      resetAllTimers();
      swal
        .fire({
          title: "Notice",
          html: `${msg}`,
          icon: "info",
          allowOutsideClick: false,
          showCancelButton: false,
          confirmButtonText: "Click here to start again",
        })
        .then((result) => {
          if (result.isConfirmed) {
            window.location.href = basePath;
          }
        });
    };

    const monitorJwtToken = () => {
      clearTimeout(jwtTimeout);
      // avoid checking for jwt expiring when modal is up
      if (swal.isVisible()) {
        //console.log("sweet alert pop up is open");
        return;
      }
      let storedTimestamp = localStorage.getItem(JWT_TOKEN_EXP_DATE_NAME);
      storedTimestamp ??= sessionStorage.getItem(JWT_TOKEN_EXP_DATE_NAME);
      //console.log('storedTimestamp: ' + storedTimestamp);
      if (!storedTimestamp) {
        // If there's no session expiration data, recheck in 10 seconds
        jwtTimeout = setTimeout(monitorJwtToken, 10000);
        return;
      }
      storedTimestamp = Number(storedTimestamp) / 1000;
      // Get current time in seconds
      let currentTime = Math.floor(Date.now() / 1000);
      // console.log(basePath);
      // console.log(currentTime - storedTimestamp);
      if (currentTime >= storedTimestamp) {
        // console.log('session expired');
        // Redirect to session expired page

        // console.log('token expired');
        showSessionExpiredPopup("Your token has Expired.");
      } else {
        // Convert to milliseconds
        const timeLeft = (storedTimestamp - currentTime) * 1000;
        // console.log('timeLeft: ' + timeLeft);
        jwtTimeout = setTimeout(monitorJwtToken, timeLeft); // Re-check at expiration time
      }
    };

    const startIdleTimer = () => {
      // give one minute warning
      /*
          timeout - warningtime = 60000

          formula:
          if timeout > 60000
              warningTime = 60000
          else
              warningTime = timeout - 10000
      */
      let getJwt = localStorage.getItem(JWT_TOKEN_NAME);
      getJwt ??= sessionStorage.getItem(JWT_TOKEN_NAME);

      // make sure to only run idletimer if user is authenticated
      if (!getJwt) {
        // check again in 10 seconds (I arbitrarily chose 10 seconds)
        startIdleTimerTimeout = setTimeout(startIdleTimer, 10000);
        return;
      }

      // make sure we have at least 15 seconds to deal with
      idleTimeOut = Math.max(15000, idleTimeOut);
      // 5 minutes
      const SESSION_DISPLAY_TIME_IN_MILLISECONDS = 300000;
      const warningTime =
        idleTimeOut > SESSION_DISPLAY_TIME_IN_MILLISECONDS
          ? SESSION_DISPLAY_TIME_IN_MILLISECONDS
          : idleTimeOut - 10000;

      function restartIdleTimer() {
        const showWarning = async () => {
          let timeLeftInSeconds = warningTime / 1000;
          // console.log('showing warning');
          // this is a check to avoid this popup if another pop up is already up such as the token expired pop up
          if (swal.isVisible()) {
            resetAllTimers();
            return;
          }
          let result = await swal.fire({
            title: "Session Timeout Warning",
            html: `You have been idle. Redirecting in <b><span id="countdown">${timeLeftInSeconds}</span></b> seconds.`,
            icon: "warning",
            // in milliseconds
            timer: warningTime,
            allowOutsideClick: false,
            showCancelButton: true,
            confirmButtonText: "Stay Logged In",
            cancelButtonText: "Logout",
            didOpen: () => {
              const countdownEl = document.getElementById("countdown");
              countDownInterval = setInterval(() => {
                timeLeftInSeconds--;
                //console.log(timeLeftInSeconds);
                countdownEl.innerText = timeLeftInSeconds;
                if (timeLeftInSeconds <= 0) {
                  swal.close();
                  showSessionExpiredPopup("Your session has Expired.");
                }
              }, 1000);
            },
          });
          if (result.isConfirmed) {
            restartIdleTimer();
          } else {
            showSessionExpiredPopup("Your session has Expired.");
          }
        };

        clearInterval(countDownInterval);
        clearTimeout(idleTimer);
        idleTimer = undefined;
        // show it in (timeout - warningTime milliseconds)
        // for example, if there's a total of 20 seconds of allowed idle time,
        // and the warning time is 15 seconds left then the pop up will appear 5 seconds after 20 second timer has started
        idleTimer = setTimeout(showWarning, idleTimeOut - warningTime);
      }

      // Reset timer on user activity
      window.addEventListener("mousemove", () => {
        //console.log('moved mouse');
        // avoid resetting timer while any of the swal pop ups are open
        if (!swal.isVisible()) {
          restartIdleTimer();
        }
      });
      window.addEventListener("keydown", () => {
        //console.log('keydowned');
        if (!swal.isVisible()) {
          restartIdleTimer();
        }
      });
      window.addEventListener("scroll", () => {
        //console.log('scrolled');
        if (!swal.isVisible()) {
          restartIdleTimer();
        }
      });
      window.addEventListener("click", () => {
        //console.log('clicked');
        if (!swal.isVisible()) {
          restartIdleTimer();
        }
      });

      // Start the timer initially
      restartIdleTimer();
    };

    if (!id || ![1, 2].includes(Number(id))) {
      return;
    }
    switch (Number(id)) {
      case 1:
        monitorJwtToken();
        break;
      case 2:
        startIdleTimer();
        break;
    }
  },
};

UI = {
  // DEPRECATED because of blazor @onkeydown attribute
  // closes the multiselect when user presses escape
  // registerEscapeKeyHandler: (dotNetObject) => {
  //   document.addEventListener("keydown", function (event) {
  //     if (event.key === "Escape") {
  //       // console.log('escape pressed');
  //       dotNetObject.invokeMethod("CloseDropdown");
  //     }
  //   });
  // },
};

FILE = {
  downloadFile: (base64Data, contentType, fileName) => {
    const blob = new Blob(
      [Uint8Array.from(atob(base64Data), (c) => c.charCodeAt(0))],
      { type: contentType },
    );
    const url = URL.createObjectURL(blob);

    // Create a temporary anchor element
    const a = document.createElement("a");
    a.href = url;
    a.download = fileName;
    document.body.appendChild(a);

    // Trigger download and clean up
    a.click();
    URL.revokeObjectURL(url);
    document.body.removeChild(a);
  },

  saveAsFile: (filename, fileContent) => {
    var link = document.createElement("a");
    link.download = filename;
    // mime-types: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
    link.href =
      "data:text/plain;charset=utf-8," + encodeURIComponent(fileContent);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },
};
