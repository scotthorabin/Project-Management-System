$(document).ready(function () {
    $('.expandBtn').click(function (e) {
        e.preventDefault();
        var $infoBox = $(this).closest('.infoBoxCoordinator');

        // Collapse all other info boxes
        $('.infoBoxCoordinator').not($infoBox).removeClass('expanded').find('.coordinatorContent').slideUp(300);
        $('.expandIcon').css({ 'transform': 'rotate(270deg)' });

        // Toggle the clicked info box
        $infoBox.toggleClass('expanded');

        if ($infoBox.hasClass('expanded')) {
            $(this).find('.expandIcon').css({ 'transform': 'rotate(90deg)' });
            $infoBox.find('.coordinatorContent').stop(true, true).slideDown(300);
        } else {
            $(this).find('.expandIcon').css({ 'transform': 'rotate(270deg)' });
            $infoBox.find('.coordinatorContent').stop(true, true).slideUp(300);
        }
    });

    $('.topicCheckbox').on('change', function () {
        // Uncheck all checkboxes
        $('.topicCheckbox').not(this).prop('checked', false);
    });
});
