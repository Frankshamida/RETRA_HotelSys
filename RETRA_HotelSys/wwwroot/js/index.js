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