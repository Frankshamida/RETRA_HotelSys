$(document).ready(function () {
    var header = $("#mainHeader");
    var scrollThreshold = 100;

    $(window).scroll(function () {
        if ($(this).scrollTop() > scrollThreshold) {
            header.addClass("scrolled");
        } else {
            header.removeClass("scrolled");
        }
    });

    // Add animation delays programmatically
    $('.animate-fade-in').each(function (index) {
        var delay = $(this).css('--delay') || (index * 0.1 + 0.1);
        $(this).css('animation-delay', delay + 's');
    });
});


/*---------------------------------------------------------------------- GuestHome.js------------------------------------------------------------------------ */

    // Generate random color for avatar
    function getRandomColor() {
        const colors = ['#e74c3c', '#3498db', '#2ecc71', '#f39c12', '#9b59b6', '#1abc9c'];
    return colors[Math.floor(Math.random() * colors.length)];
    }

    // Set avatar color and initials
    document.addEventListener('DOMContentLoaded', function() {
        // Set random color for avatar
        const avatar = document.getElementById('userAvatar');
    avatar.style.backgroundColor = getRandomColor();

    // Logout functionality
    document.getElementById('logoutBtn').addEventListener('click', function(e) {
        e.preventDefault();
    // Add your logout logic here
    alert('Logging out...'); // Replace with actual logout code
            // window.location.href = '/Account/Logout';
        });

        // No need for the demo user object since we're using model data
    });

/*------------------------------------------------------------------------------------------------------------------------------------------------------------ */


/*--------------------------------------------------------------SIDEBAR FUNCTION------------------------------------------------------------------------------- */

        // Set minimum date for check-in to today
        document.getElementById('CheckInDate').min = new Date().toISOString().split('T')[0];

        // Set minimum date for check-out to tomorrow
        document.getElementById('CheckOutDate').min = new Date(Date.now() + 86400000).toISOString().split('T')[0];

        // When check-in date changes, update check-out min date
        document.getElementById('CheckInDate').addEventListener('change', function() {
            const checkInDate = new Date(this.value);
        const nextDay = new Date(checkInDate);
        nextDay.setDate(nextDay.getDate() + 1);

        const checkOutField = document.getElementById('CheckOutDate');
        checkOutField.min = nextDay.toISOString().split('T')[0];

        // If current check-out is before new min, reset it
        if (new Date(checkOutField.value) < nextDay) {
            checkOutField.value = nextDay.toISOString().split('T')[0];
            }
        });

/*------------------------------------------------------------------------------------------------------------------------------------------------------------ */


if (performance.navigation.type === 2) {
    // This page was loaded from cache (back button)
    location.reload(true); // Force reload from server
}


$(document).ready(function () {
    $('#changePasswordForm').submit(function (e) {
        e.preventDefault();

        var newPassword = $('#newPassword').val();
        var confirmPassword = $('#confirmPassword').val();

        if (newPassword !== confirmPassword) {
            alert('New password and confirmation do not match!');
            return false;
        }

        if (newPassword.length < 6) {
            alert('Password must be at least 6 characters long!');
            return false;
        }

        // If validation passes, submit the form
        this.submit();
    });
});