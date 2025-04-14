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

docu
});
/*------------------------------------------------------------------------------------------------------------------------------------------------------------ */
