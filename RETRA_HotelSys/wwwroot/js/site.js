/* SHRINK NAVBAR */
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
        });
/* SHRINK NAVBAR */