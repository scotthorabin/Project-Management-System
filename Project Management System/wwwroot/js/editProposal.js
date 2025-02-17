$(document).ready(function () {
    $('#topicDropdown').change(function () {
        var topicVal = $(this).val();
        if (topicVal !== '') {
            $('.editSection').slideDown(400);  // Slide down animation to show the section
        } else {
            $('.editSection').slideUp(400);  // Slide up animation to hide the section
        }
    });
});
