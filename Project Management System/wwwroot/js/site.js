// Changes fields to topic of users choosing
$(document).ready(function () {
    // Event listener for dropdown change
    $('#topicDropdown').change(function () {
        // Get the selected option
        var selectedOption = $(this).children('option:selected');

        // Retrieve data attributes
        var topicName = selectedOption.data('name');
        var topicDescription = selectedOption.data('description');
        var supervisorID = selectedOption.data('supervisor');
        var markerID = selectedOption.data('marker');

        // Set values to input fields
        $('#topicName').val(topicName);
        $('#topicDescription').val(topicDescription);
        $('#topicSupervisor').val(supervisorID);
        $('#topicMarker').val(markerID);

        // Show the edit section if it's hidden
        $('.editSection').show();
    });

$(document).ready(function () {
    // Assuming '#toggle' is the ID of the button that will be clicked to open and close the sidebar
    const sidebar = $('#sidebar');
    const toggle = $('#toggle'); // Single button for toggling the sidebar
    const divider = $('.header--divider--desktop');
    const uocLogo = $('.header--uocLogo--standard');
    const userAccount = $('.header--userAccount--standard');

    toggle.click(function () {
        // Check if sidebar is open by checking its left style property
        if (sidebar.css('left') === '0px') {
            // If it's open, close it
            sidebar.css('left', '-306px');
            toggle.css('left', '10px');
            divider.css('left', '90px');
            uocLogo.css('left', '110px');
            uocLogo.show();
            userAccount.css('opacity', '1');
            userAccount.toggleClass('expanded');
        } else {
            // If it's closed, open it
            sidebar.css('left', '0');
            toggle.css('left', '316px');
            divider.css('left', '406px');
            uocLogo.css('left', '426px');
            uocLogo.hide();
            userAccount.css('opacity', '0.5');
            userAccount.toggleClass('expanded');
        }
        // Toggle the button's appearance as well
        toggle.toggleClass('button-open button-close');
    });

    $('.header--accountBtn').click(function () {
        var $dropdownMenu = $('.dropdown-menu');
        if ($dropdownMenu.is(':visible')) {
            $dropdownMenu.slideUp(); // Animates the hiding of the dropdown
        } else {
            $dropdownMenu.slideDown(); // Animates the showing of the dropdown
        }
    });

    document.getElementById('logout').addEventListener('click', function (event) {
        event.preventDefault(); // Prevent the default action of following the link
        document.getElementById('logoutForm').submit(); // Submit the form
        // Show a popup notification
        alert("You have been logged out.");
    });

    try {
        throw new Error("Custom exception message");
    } catch (error) {
        // Handle the exception here, or you can choose not to handle it if you don't want to display any message to the user
        console.error("An exception occurred:", error);
    }

    function filterFunction1() {
        document.getElementById("myDropdown").classList.toggle("show");
    }
  
    });

});

// Clears search filter and resets so all topics are viewable again
function clearSearch() {
    // Clear search input
    document.querySelector('input[name="Search"]').value = '';

    // Redirect to the index page to clear the search query
    window.location.href = '/Index';
}

function showError(errorElement, errorMessage) {
    document.querySelector("." + errorElement).classList.add("display-error");
    document.querySelector("." + errorElement).innerHTML = errorMessage;
}

function showError(errorElement, errorMessage) {
    let error = document.querySelector("." + errorElement);
    error.textContent = errorMessage;
    error.classList.add("display-error"); // Show error message
}

function clearError(errorElement) {
    let error = document.querySelector("." + errorElement);
    error.textContent = ""; // Clear error message
    error.classList.remove("display-error"); // Hide error message
}

let form = document.forms["create-topic-form"];
form.onsubmit = function (event) {
    if (form.topicName.value === "") {
        showError("topicName-error", "Please enter topic name");
        event.preventDefault();
    }

    if (form.topicDescription.value === "") {
        showError("topicDescription-error", "Please enter topic description!");
        event.preventDefault();
    }
};


