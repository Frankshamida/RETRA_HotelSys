 $(document).ready(function() {
            var header = $("#mainHeader");
    var scrollThreshold = 100;

    $(window).scroll(function() {
                if ($(this).scrollTop() > scrollThreshold) {
        header.addClass("scrolled");
                } else {
        header.removeClass("scrolled");
                }
            });

    // Add animation delays programmatically
    $('.animate-fade-in').each(function(index) {
                var delay = $(this).css('--delay') || (index * 0.1 + 0.1);
    $(this).css('animation-delay', delay + 's');
     });
});

/*----------------------------------------NOTIFICATION FOR SUCCESS REGISTER-------------------------------------- */

$(document).ready(function () {
    // Fade out and remove TempData alert after 3 seconds
    $('#tempDataAlert').delay(3000).fadeOut(500, function () {
        $(this).remove();
    });

    // Fade out and remove ViewBag alert after 3 seconds
    $('#viewBagAlert').delay(3000).fadeOut(500, function () {
        $(this).remove();
    });
});
/*-------------------------------------------------------------------------------------------------------------- */

// OTP Countdown Timer (5 minutes)
let timeLeft = 300; // 5 minutes in seconds
const timerElement = document.getElementById('countdown');

const countdown = setInterval(() => {
    timeLeft--;
    const minutes = Math.floor(timeLeft / 60);
    const seconds = timeLeft % 60;

    timerElement.textContent = `${minutes.toString().padStart(2, '0')}:${seconds.toString().padStart(2, '0')}`;

    if (timeLeft <= 0) {
        clearInterval(countdown);
        timerElement.textContent = "Expired";
        timerElement.style.color = "#dc3545";
        document.querySelector('button[type="submit"]').disabled = true;
    }
}, 1000);


/*-------------------------------------------------------------------------------------------------------------- */

document.addEventListener('DOMContentLoaded', function () {
    const passwordInput = document.getElementById('passwordInput');
    const strengthBar = document.getElementById('strengthBar');
    const strengthText = document.getElementById('strengthText');

    passwordInput.addEventListener('input', function () {
        const strength = checkPasswordStrength(this.value);
        updateStrengthMeter(strength);
    });

    function checkPasswordStrength(password) {
        // Criteria checks
        const hasMinLength = password.length >= 8;
        const hasUpperCase = /[A-Z]/.test(password);
        const hasLowerCase = /[a-z]/.test(password);
        const hasNumbers = /\d/.test(password);
        const hasSpecial = /[!@#$%^&*(),.?":{}|<>]/.test(password);

        // Calculate strength (0-4)
        let strength = 0;
        if (hasMinLength) strength++;
        if (hasUpperCase) strength++;
        if (hasLowerCase) strength++;
        if (hasNumbers) strength++;
        if (hasSpecial) strength++;

        return strength;
    }

    function updateStrengthMeter(strength) {
        const colors = ['#dc3545', '#ffc107', '#ffc107', '#28a745', '#28a745'];
        const texts = ['Very Weak', 'Weak', 'Moderate', 'Strong', 'Very Strong'];

        strengthBar.style.width = `${(strength / 4) * 100}%`;
        strengthBar.style.background = colors[strength] || '#dc3545';
        strengthText.textContent = texts[strength] || 'Very Weak';
    }
});