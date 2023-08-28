// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function (event) {

    const showNavbar = (toggleId, navId, bodyId, headerId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId)

// Validate that all variables exist
        if (toggle && nav && bodypd && headerpd) {
            toggle.addEventListener('click', () => {
                // show navbar
                nav.classList.toggle('show')
                // change icon
                toggle.classList.toggle('bx-x')
                // add padding to body
                bodypd.classList.toggle('body-pd')
                // add padding to header
                headerpd.classList.toggle('body-pd')
            })
        }
    }

    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header')

    /*===== LINK ACTIVE =====*/
    const linkColor = document.querySelectorAll('.nav_link')

    function colorLink() {
        if (linkColor) {
            linkColor.forEach(l => l.classList.remove('active'))
            this.classList.add('active')
        }
    }

    linkColor.forEach(l => l.addEventListener('click', colorLink))

    // Your code to run since DOM is loaded and ready
});

$("#btnLogin").click(function(event) {

    //Fetch form to apply custom Bootstrap validation
    var form = $("#formLogin")

    if (form[0].checkValidity() === false) {
        event.preventDefault()
        event.stopPropagation()
    }

    form.addClass('was-validated');
});

// // On dropdown open
// $(document).on('shown.bs.dropdown', function(event) {
//     var dropdown = $(event.target);
//
//     // Set aria-expanded to true
//     dropdown.find('.dropdown-menu').attr('aria-expanded', true);
//
//     // Set focus on the first link in the dropdown
//     setTimeout(function() {
//         dropdown.find('.dropdown-menu li:first-child a').focus();
//     }, 10);
// });
//
// // On dropdown close
// $(document).on('hidden.bs.dropdown', function(event) {
//     var dropdown = $(event.target);
//
//     // Set aria-expanded to false        
//     dropdown.find('.dropdown-menu').attr('aria-expanded', false);
//
//     // Set focus back to dropdown toggle
//     dropdown.find('.dropdown-toggle').focus();
// });
