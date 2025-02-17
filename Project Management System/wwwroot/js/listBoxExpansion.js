$(document).ready(function () {
    // This function runs when the document is fully loaded
    console.log("Page loaded and script running"); // Check if the script runs at all

    // Event handler for clicking on elements with class '.expandTopic'
    $('.expandTopic').click(function (e) {
        e.preventDefault();
        console.log("Button clicked");  // Check if the event is captured

        // Toggle the 'expanded' class on the closest '.topicBox' element
        $(this).closest('.topicBox').toggleClass('expanded');

        // Debugging: Check if the 'expanded' class is applied
        console.log("Toggle class applied: ", $(this).closest('.topicBox').hasClass('expanded'));

        // Update button title based on whether the topic box is expanded or collapsed
        if ($(this).closest('.topicBox').hasClass('expanded')) {
            $(this).attr('title', 'Collapse this topic for fewer details');
        } else {
            $(this).attr('title', 'Expand this topic for more details');
        }
    });
});

// Function to handle selecting a course
function selectCourse(courseName) {
    // Redirect to the same page with the selected course name as a query parameter
    window.location.href = '/?course=' + encodeURIComponent(courseName);
}
