
// closes the multiselect when user presses escape
window.dropdownInterop = {
    registerEscapeKeyHandler: function (dotNetObject) {
        document.addEventListener('keydown', function (event) {
            if (event.key === 'Escape') {
                console.log('escape pressed');
                dotNetObject.invokeMethodAsync('CloseDropdown');
            }
        });
    }
};

window.downloadFile = (base64Data, contentType, fileName) => {
    const blob = new Blob([Uint8Array.from(atob(base64Data), c => c.charCodeAt(0))], { type: contentType });
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
}

window.saveAsFile = function(filename, fileContent)
{
    var link = document.createElement('a')
    link.download = filename
    // mime-types: https://developer.mozilla.org/en-US/docs/Web/HTTP/Basics_of_HTTP/MIME_types/Common_types
    link.href = 'data:text/plain;charset=utf-8,'+encodeURIComponent(fileContent)
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
}