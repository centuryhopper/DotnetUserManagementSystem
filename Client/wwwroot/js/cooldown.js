
const cooldownObj = () =>
{
    const MAX_VERIFY_COUNT = 5
    const COOLDOWN_SECONDS = 5
    let countsSoFar = 0
    let attemptsNotice = ''
    let erMsg = ''
    let coolDownInterval = null

    const reset = () =>
    {
        countsSoFar = 0
        attemptsNotice = ''
        $('#attempts-notice').text('')
        $('#cool-down-btn').prop('disabled', false)
        $('#cooldown-timer').text('')
        $('#errMsg').text('')
        erMsg = ''
    }

    return {
        MAX_VERIFY_COUNT,
        COOLDOWN_SECONDS,
        generateAttemptsNotice()
        {
            countsSoFar += 1
            // make sure we don't display a count that goes over our MAX_VERIFY_COUNT
            attemptsNotice = `${Math.min(countsSoFar, MAX_VERIFY_COUNT)} of ${MAX_VERIFY_COUNT} attempts`
            $('#attempts-notice').text(attemptsNotice)

            if (countsSoFar >= MAX_VERIFY_COUNT)
            {
                let time = COOLDOWN_SECONDS
                if (!coolDownInterval)
                {
                    $('#cool-down-btn').prop('disabled', true)
                    coolDownInterval = setInterval(() =>
                    {
                        if (time <= 0)
                        {
                            clearInterval(coolDownInterval)
                            coolDownInterval = null
                            reset()
                            return
                        }
                        $('#cooldown-timer').text(`Please try again after ${time--} seconds`)
                    }, 1000);
                }
            }
        },
        getErrMsg()
        {
            return erMsg
        },
        setErrMsg(val)
        {
            erMsg = val
            $('#errMsg').text(val)
        },
    }
}


window.cooldown = cooldownObj()


