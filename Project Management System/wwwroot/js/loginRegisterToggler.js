$(document).ready(function () {

    // Hides the element with ID "newDepartment"
    $('#newDepartment').hide();

    var lastForm = '@Html.Raw(Json.Serialize(ViewBag.LastForm))';

    // Checks if value of lastForm is "Register"
    if (lastForm === "Register") {
        // Hides element 'hide' and shows the 'register' form
        $('.loginForm').hide();
        $('.registerForm').show();
        // CSS
        $('.staffBtn').css('border-bottom', '2px solid red');
        $('.body--register--staffToggler').show();
    } else {
        // else, hide 'register' form and show 'login' form
        $('.registerForm').hide();
        $('.loginForm').show();

        $('.studentBtn').css('border-bottom', '2px solid red');
        $('.body--register--staffToggler').hide();
    }

    $('.body--register--inputToggle').click(function () {
        if ($('.loginForm').is(':visible')) {
            toggleToRegister();
        } else {
            toggleToLogin();
        }
    });

    $('.studentBtn').click(function () {
        // Toggle to student registration form
        $('.registerFormStaff').slideUp(400);
        $('.registerForm').slideDown(400);
        $(this).css('border-bottom', '2px solid red');
        $('.staffBtn').css('border', 'none');
    });

    $('.staffBtn').click(function () {
        // Toggle to staff registration form
        $('.registerForm').slideUp(400);
        $('.registerFormStaff').slideDown(400);
        $(this).css('border-bottom', '2px solid red');
        $('.studentBtn').css('border', 'none');
    });

    function toggleToRegister() {
        $('.loginForm').slideUp(600, function () {
            $('.registerForm').slideDown(800);
            $('.body--register--staffToggler').show();
        });
        updateToggleText(true);
    }

    function toggleToLogin() {
        $('.registerForm').slideUp(600);
        $('.registerFormStaff').slideUp(600, function () {
            $('.loginForm').slideDown(600);
            $('.body--register--staffToggler').hide();
        });
        updateToggleText(false);
    }

    function updateToggleText(isRegistering) {
        if (isRegistering) {
            $('.body--login--newUser').text('Already have an account? Click here to login:');
            $('.body--login--container').css('min-height', '950px');
            $('.body--register--inputToggle').text('Login');
        } else {
            $('.body--login--newUser').text('New here? Register below:');
            $('.body--login--container').css('min-height', '650px');
            $('.body--register--inputToggle').text('Register');
        }
    }


    $('#newSchool').change(function () {
        var schoolValue = $(this).val();
        if (schoolValue !== 'null') {
            $('#newDepartment').prop('disabled', false).slideDown(400);  // Slide down animation to show the department select
        } else {
            $('#newDepartment').prop('disabled', true).val('null').slideUp(400);  // Slide up animation to hide the department select
        }
    });
});


function displayAlert(message) {
    alert(message);
    window.location.href = '/Account/Login';
}
