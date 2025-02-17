$(document).ready(function () {
    $('.infoBox').click(function (event) {
        event.stopPropagation(); // Prevent click from bubbling to document
        var wasActive = $(this).hasClass('active');

        // Deactivate all other infoBoxes and reset their styles
        $('.infoBox').removeClass('active').css({
            'transform': 'scale(1)',
            'z-index': '1'
        }).find('.content').slideUp(200).css('opacity', '0');

        $('.infoBox').find('.infobox--icon').css({
            'opacity': '1',
            'display': 'block' // Ensure icons are visible when inactive
        });

        // Toggle this infoBox based on its previous state
        if (!wasActive) {
            $(this).addClass('active').css({
                'transform': 'scale(1.5)',
                'z-index': '998'
            }).find('.content').slideDown(200).css('opacity', '1');

            $(this).find('.infobox--icon').css({
                'opacity': '0',
                'display': 'none' // Hide icon when active
            });
        } else {
            $(this).removeClass('active').css({
                'transform': 'scale(1)',
                'z-index': '1'
            }).find('.content').slideUp(200).css('opacity', '0');

            $(this).find('.infobox--icon').css({
                'opacity': '1',
                'display': 'block' // Ensure icon reappears when not active
            });
        }
    });

    $('.editInfo').click(function (event) {
        event.stopPropagation(); // Stops the click event from bubbling up to the parent infoBox
    });

    $(document).click(function () {
        $('.infoBox.active').removeClass('active').css({
            'transform': 'scale(1)',
            'z-index': '1'
        }).find('.content').slideUp(200).css('opacity', '0');

        $('.infoBox').find('.infobox--icon').css({
            'opacity': '1',
            'display': 'block'
        });
    });
});
