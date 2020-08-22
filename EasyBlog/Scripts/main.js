function Login() {
    var form = document.getElementsByClassName('needs-validation')[0];

    if (form.checkValidity() === false) {
        event.preventDefault();
        event.stopPropagation();
    }

    form.classList.add('was-validated');
    let data = {};
    data.email = $('#login-email').val();
    data.password = $('#login-password').val();

    if (data.email.includes('@') && data.email.length > 1 && data.password.length > 0) {
        var response = AjaxCall("User","Authorization", data);
        if (response != 'success') {
            document.getElementById('login-response').innerHTML = response;
            document.getElementsByClassName('alert-danger')[0].style.display = 'block';
        } else {
            if (window.location.href.includes("Login") || window.location.href.includes("Authorization")) {
                let url = window.location.href;
                url = url.replace('Authorization', 'Entry');
                url = url.replace('Login', 'Entry');
                window.location.href = url;
            } else {
                window.location.href = "User/Entry";
            }
        }
    }
}

function ConfirmEmail() {
    let data = {};
    data.email = $('#email-confirm').val();
    window.email = data.email;

    var response = AjaxCall("User", "ConfirmEmail", data);
    var parsedResult = response.split(",");

    if (parsedResult.length ==  1) {
        document.getElementById('confirm-email-response').innerHTML = response;
        document.getElementsByClassName('alert-danger')[1].style.display = 'block';
    }
    else {
        document.getElementsByClassName('alert-danger')[1].style.display = 'none';
        HandshakeOptions(parsedResult);
    }
}

//TODO
function HandshakeOptions(option) {
    console.log(option);
    if (option == 'One') {

    } else {
        document.getElementById('email-checkbox-value').innerHTML = option[1] + " send message.";
        document.getElementById('phone-checkbox-value').innerHTML = option[0] + " send mail.";
        document.getElementById('confirm-email-button').style.display = 'none';
        document.getElementById('send-code-button').style.display = 'block';
        document.getElementsByClassName('email-phone-checkbox')[0].style.display = 'block';
    }
}


//TODO
function HandshakeTimer() {
    $('#email-confirm').val("");
    if (window.lock != null) {
        return 0;
    }

    let seconds = 60;
    let timer;
    if (document.getElementById('email-checkbox').checked == true) {
        window.lock = true;
        var data = {};
        data.method = 'email';
        data.sendTo = window.email;
        console.log(document.getElementById('email-checkbox'));
        var response = AjaxCall("User", "Handshake", data);

    }

    if (document.getElementById('phone-checkbox').checked == true) {
        console.log('osman');

    }

    if (!timer) {
        timer = window.setInterval(function () {
            if (seconds < 60) {
                document.getElementById("timer").style.width = (seconds * 100 / 60).toString() + "%";
                if (seconds.toString().length == 1) {
                    document.getElementById("timer").innerHTML = "0:0" + seconds;
                }
                else {
                    document.getElementById("timer").innerHTML = "0:" + seconds;
                }
            }
            if (seconds > 0) {
                seconds--;
            } else {
                location.reload();
                clearInterval(timer);
            }
        }, 1000);
    }

    document.getElementById("timer").style.width = "100%";
    document.getElementById("timer").innerHTML = "1:00"
}

function CheckCode() {
    var data = {};
    data.code = document.getElementById('handshake-code').value;
    var response = AjaxCall("User", "CheckCode", data);
    if (response == 'success') {
        window.lock = null;
        if (window.location.href.includes("User")) {
            let url = window.location.href;
            url = url.replace('Login', 'Reset');
            window.location.href = url;
        } else
        {
            window.location.href = "User/Reset";
        }
        console.log(response);
    }
    else {
        window.lock = null;
        console.log(response);
    }
    $('#handshake-code').val("");
}


function RefreshPage() {
    location.reload();
}

function Register() {
    let data = {};
    data.name = $('#first-name').val();
    data.surname = $('#last-name').val();
    data.email = $('#email').val();
    data.phone = $('#phone').val();
    data.password = $('#password').val();
    data.confirmPassword = $('#confirm-password').val();

    var form = document.getElementsByClassName('needs-validation')[0];

    if (form.checkValidity() === false) {
        event.preventDefault();
        event.stopPropagation();
    }

    form.classList.add('was-validated');
    if (document.getElementById('strong-password').style.display == 'block') {
        document.getElementById('register-response').innerHTML = "Please pick valid password";
        document.getElementsByClassName('alert-danger')[0].style.display = 'block';
    }
    else if (!(data.name == '' || data.surname == '' || data.email == '' || data.password == '' || data.confirmPassword == '' ||
        data.name.trim() == '' || data.surname.trim() == '' || data.email.trim() == '' || data.password.trim() == '' || data.confirmPassword.trim() == '')) {
        var response = AjaxCall("User", "CreateUser", data);
        if (response.Type == 'success') {
            document.getElementsByClassName('alert-danger')[0].style.display = 'none';
            CreateDialog(response.Type, response.Type, response.Message, '', '', 'window.history.back()');
        }
        else {
            document.getElementById('register-response').innerHTML = response;
            document.getElementsByClassName('alert-danger')[0].style.display = 'block';
        }
    } else {
        document.getElementById('register-response').innerHTML = "Invalid parameter";
        document.getElementsByClassName('alert-danger')[0].style.display = 'block';
    }
}
//TODO
window.addEventListener('load', (event) => {
    if (window.location.href.includes("Register") || window.location.href.includes("Settings") || window.location.href.includes("Reset")) {
        document.getElementById('password').addEventListener('input', function () {
            document.getElementById('strong-password').style.display = 'block';
            const password = document.querySelector('#password');
            password.addEventListener('input', function (event) {
                var password = event.target.value;
                var format = /[ `!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?~]/;
                var flagList = [["char-limit", false], ["uppercase", false], ["lowercase", false], ["number", false], ["special", false]];

                for (var index = 0; index < password.length; index++) {
                    if (password[index] == password[index].toLowerCase() && !Number.isInteger(parseInt(password[index]))) {
                        flagList[2][1] = true;
                    }
                    if (password[index] == password[index].toUpperCase() && !Number.isInteger(parseInt(password[index]))) {
                        flagList[1][1] = true;
                    }
                    if (Number.isInteger(parseInt(password[index]))) {
                        flagList[3][1] = true;
                    }
                    if (format.test(password)) {
                        flagList[4][1] = true;
                    }
                    if (password.length == 8 || password.length > 8) {
                        flagList[0][1] = true;
                    }
                }
                ChangePasswordCondition(flagList);
                CheckPasswordConditions();
            });
        })
    }
    if (window.location.href.includes("CreatePage")) {
        var response = AjaxCall("Admin", "HasBlog", {});
        if (response.Type != 'success') {
            CreateDialog(response.Type, response.Type, response.Message, 'Continue', 'DeletePage()', 'window.history.back()');
        }
        document.getElementById('submit').onclick = function () {
            var formdata = new FormData();
            Array.from(document.getElementsByClassName('custom-file-input')).forEach(function (fileInput) {
                for (i = 0; i < fileInput.files.length; i++) {
                    formdata.append(fileInput.files[i].name, fileInput.files[i]);
                }
            });
            //TODO
            //Response check
            var xhr = new XMLHttpRequest();
            xhr.open('POST', '/Admin/UploadImages', false);
            xhr.send(formdata);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    CreateDialog('success', 'Success', 'Your page is ready!', '', '', 'ReturnHome()');
                } else if (xhr.status == 500) {
                    CreateDialog('error', 'Error', 'System upload file error!', '', '', 'ReturnHome()');
                }
            };
            if (xhr.readyState == 4 && xhr.status == 200) {
                CreateDialog('success', 'Success', 'Your page is ready!', '', '', 'window.history.back()');
            } else if (xhr.status == 500) {
                CreateDialog('error', 'Error', 'System upload file error!', '', '', 'window.history.back()');
            }
            console.log(xhr.response)
        }    
    }
    if (window.location.href.includes("UpdatePage")) {
        TableEventListener();
    }
});

function ChangePasswordCondition(array) {
    array.forEach(function (element) {
        if (element[1]) {
            document.getElementById(element[0] + '-icon').innerHTML = '&#x2714;';
            var element = document.getElementById(element[0] );
            element.className = element.className.replace(/\bred\b/g, "green");
        }
        else {
            document.getElementById(element[0] + '-icon').innerHTML = '&#x2716;';
            var element = document.getElementById(element[0]);
            element.className = element.className.replace(/\bgreen\b/g, "red");
        }

    });
}

function CheckPasswordConditions() {
    var numberOfElement = document.getElementsByClassName('green').length;
    if (numberOfElement == 5) {
        document.getElementById('strong-password').style.display = 'none';
    }
    else {
        document.getElementById('strong-password').style.display = 'block';
    }
}

function CreateDialog(type, title, body, button, action, defaultAction){
    const color = GetDialogColor(type);
    let myButton = '';
    if (button != '') {
        myButton = '<button type="button" class="btn btn-light" data-dismiss="modal" onclick="' + action + '" >' + button + '</button>';
    }
    let myDialog = '<div class="modal fade" id="my-dialog" tabindex="-1" role="dialog" aria-labelledby="my-dialog" aria-hidden="true">\
                        <div class="modal-dialog">\
                            <div class="modal-content">\
                                <div class="modal-header" style="background:'+ color + '">\
                                    <h4 class="modal-title" id="myModalLabel">'+ FirstLetterUpperCase(title) +'</h4>\
                                    <button type="button" class="close" data-dismiss="modal" onclick="' + defaultAction + '" aria-label="Close">\
                                        <span aria-hidden="true">&times;</span>\
                                    </button>\
                                </div>\
                                <div class="modal-body">\
                                    <p>'+ body +'</p>\
                                    <div class="modal-footer">\
                                        <button type="button" class="btn btn-light" data-dismiss="modal" onclick="' + defaultAction + '">Close</button>\
                                        '+ myButton +'\
                                    </div>\
                                </div>\
                            </div>\
                        </div>\
                    </div>';

    document.getElementById('dialog').innerHTML = myDialog;
    document.querySelector('#trigger-dialog').click();
}

function GetDialogColor(type) {
    if (type == 'error') {
        return '#dc3545';
    }
    else if (type == 'success') {
        return '#28a745';
    }
    else if (type == 'warning') {
        return '#ffc107';
    }
    else {
        return '#007bff';
    }
}

function ChangeMargin() {
    let show = document.getElementsByClassName('show').length;
    if (show == 1) {
        setTimeout(function () {
            document.getElementsByClassName('active-page')[0].style.marginRight = '120px';
        }, 350);
    } else {
        document.getElementsByClassName('active-page')[0].style.marginRight = '0px';
    }
}

function SelectTemplate() {
    var element = event.target;
    var check = document.getElementsByClassName('active-image')[0];

    if (typeof check != 'undefined') {
        check.style.borderColor = '';
        check.style.borderWidth = "";
        check.className = '';
    }

    if (element.className != "active-image") {
        element.style.borderColor = '#5bc0de';
        element.style.borderWidth = "thick";
        element.className = 'active-image';
    }
    else {
        element.style.borderColor = '';
        element.style.borderWidth = "";
        element.className = '';
    }
}

function PreviewTemplate() {
    var url = document.getElementsByClassName('active-image')[0];
    if (typeof url != 'undefined') {
        window.open(url.id, '_blank');
    } else {
        CreateDialog('warning', 'Invalid Template', 'Please select a template.', '', '', '');
    }
}

function SaveAndContinueTemplate() {
    var selected = document.getElementsByClassName('active-image')[0];
    if (typeof selected != 'undefined') {
        CreateDialog('info', 'Confirmation', "You can't change the template in this process. Do you want to continue with this template?", 'Yes', 'SaveAndDisableTemplateSection()', '');
    } else {
        CreateDialog('warning', 'Invalid Template', 'Please select a template.', '', '', '');
    }
}

function SaveAndDisableTemplateSection() {
    var templates = document.getElementsByClassName('image-padding');
    var selected = document.getElementsByClassName('active-image')[0];
    let url = selected.id;
    var array = url.split("/");
    window.template = array[array.length - 2];

    var data = {}
    data.template = window.template;
    var response = AjaxCall("Admin", "SaveTemplate", data);
    if (response.Type == 'success') {
        document.getElementById('template-button').disabled = true;
        Array.from(templates).forEach(function (element) {
            element.onclick = '';
        });
        document.getElementById('main-settings').style.display = 'block';
        window.scrollTo(0, document.body.scrollHeight);
        SetDefaultValues();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function SetDefaultValues() {
    NavigationDefaultSections(window.template)
    if (window.template == 'Folio') {
        Folio();
    }
    else if (window.template == 'iPortfolio') {
        IPortfolio();
        Array.from(document.getElementsByClassName('extra')).forEach(function (element) {
            element.style.display = 'grid';
        });
        document.getElementById('about-extra-info').style.display = 'flex';
    }
    else if (window.template == 'MyResume') {
        MyResume();
        Array.from(document.getElementsByClassName('extra')).forEach(function (element) {
            element.style.display = 'grid';
        });
        document.getElementById('about-extra-info').style.display = 'flex';
    }
}

function IPortfolio() {
    document.getElementById('web-page-title').setAttribute('placeholder', 'iPortfolio');

    $('#main-title-color-code').val('#173b6c');
    $('#main-title-color').val('#173b6c');
    $('#main-text-color-code').val('#272829');
    $('#main-text-color').val('#272829');
    $('#main-hover-color-code').val('#37b3ed');
    $('#main-hover-color').val('#37b3ed');

    $('#nav-bar-color-code').val('#ffffff');
    $('#nav-bar-color').val('#ffffff');

    $('#home-text-color-code').val('#ffffff');
    $('#home-text-color').val('#ffffff');
}

function MyResume() {
    document.getElementById('web-page-title').setAttribute('placeholder', 'MyResume');
    $('#main-title-color-code').val('#45505b');
    $('#main-title-color').val('#45505b');
    $('#main-text-color-code').val('#272829');
    $('#main-text-color').val('#272829');
    $('#main-hover-color-code').val('#0563bb');
    $('#main-hover-color').val('#0563bb');

    $('#nav-bar-color-code').val('#f2f3f5');
    $('#nav-bar-color').val('#f2f3f5');

    $('#home-text-color-code').val('#45505b');
    $('#home-text-color').val('#45505b');
}

function Folio() {
    document.getElementById('web-page-title').setAttribute('placeholder', 'Folio');
    $('#main-title-color-code').val('#292929');
    $('#main-title-color').val('#292929');
    $('#main-text-color-code').val('#999999');
    $('#main-text-color').val('#999999');
    $('#main-hover-color-code').val('#b8a07e');
    $('#main-hover-color').val('#b8a07e');

    $('#nav-bar-color-code').val('#ffffff');
    $('#nav-bar-color').val('#ffffff');

    $('#home-text-color-code').val('#ffffff');
    $('#home-text-color').val('#ffffff');

    $('#about-background-color-code').val('#f7f7f7');
    $('#about-background-color').val('#f7f7f7');
    $('#about-frame-color-code').val('#b8a07e');
    $('#about-frame-color').val('#b8a07e');

    $('#portfolio-background-color-code').val('#ffffff');
    $('#portfolio-background-color').val('#ffffff');

    $('#blog-background-color').val('#f7f7f7');
    $('#blog-background-color-code').val('#f7f7f7');

    $('#contact-background-color').val('#ffffff');
    $('#contact-background-color-code').val('#ffffff');
}

function SaveAndDisableMainSection() {
    if (($('#web-page-logo').val() != '' || window.location.href.includes("UpdatePage")) && $('#web-page-title').val() != '') {
        var data = {};
        if (window.location.href.includes("UpdatePage")) {
            if ($('#web-page-logo').val() != '') {
                data.logo = GetFileName('#web-page-logo');
            } else {
                data.logo = null;
            }
        } else {
            data.logo = GetFileName('#web-page-logo');
        }
        data.title = $('#web-page-title').val();
        data.titleColor = $('#main-title-color').val();
        data.textColor = $('#main-text-color').val();
        data.hoverColor = $('#main-hover-color').val();
        data.socialMediaList = SocialMediaList();
        var response = AjaxCall("Admin", "SaveMainComponents", data);
        if (response.Type == 'success') {
            Array.from(document.getElementsByClassName('remove-social-media')).forEach(function (element) {
                element.setAttribute('onclick', '');
            });
            DisableInputs('main-settings');
            document.getElementById('nav-settings').style.display = 'block';
            if (!window.location.href.includes("UpdatePage")) {
                window.scrollTo(0, document.body.scrollHeight);
            }
        }
        CreateDialog(response.Type, response.Type, response.Message, '', '', '');
    }
    else {
        if ($('#web-page-logo').val() == '') {
            CreateDialog('error', 'Required parameter!', 'Web Page Logo is required!', '', '', '');
        } else {
            CreateDialog('error', 'Required parameter!', 'Web Page Title is required!','','','');
        }
    }
}

function SaveAndDisableNavigationSection() {
    if ($('#navigation-logo').val() != '' || window.location.href.includes("UpdatePage")) {
        var data = {}
        if (window.location.href.includes("UpdatePage")) {
            if ($('#navigation-logo').val() != '') {
                data.logo = GetFileName('#navigation-logo');
            } else {
                data.logo = null;
            }
        } else {
            data.logo = GetFileName('#navigation-logo');
        }
        data.barColor = $('#nav-bar-color').val();
        var navigationItems = [];
        GetSectionInformation().forEach(function (element) {
            var item = {};
            item.priority = element[0];
            item.sectionName = element[1];
            item.content = element[2];
            navigationItems.push(item);
        });
        data.navigationItems = navigationItems;
        var response = AjaxCall("Admin", "SaveNavigation", data);
        if (response.Type == 'success') {
            window.sectionQueue = GetSectionInformation();
            DisableInputs('nav-settings');
            if (!window.location.href.includes("UpdatePage")) {
                NextSection();
            }
        }
        CreateDialog(response.Type, response.Type, response.Message, '', '', '');
    } else {
        CreateDialog('error', 'Required parameter!', 'Logo is required!', '', '', '');
    }
  
}

function SaveAndDisableHomeSection() {
    if (($('#home-background').val() != '' || window.location.href.includes("UpdatePage")) && $('#home-main-text').val() != '') {
        var subTextList = [];
        let flag = false;
        Array.from(document.getElementsByClassName('sub-text')).forEach(function (sub) {
            if (sub.value == '') {
                flag = true;
            }
            subTextList.push(sub.value);
        });
        if (flag) {
            CreateDialog('error','Invalid Input','Fill all empty fields!','','','');
        } else {
            var data = {};
            if (window.location.href.includes("UpdatePage")) {
                if ($('#home-background').val() != '') {
                    data.background = GetFileName('#home-background');
                } else {
                    data.background = null;
                }
            } else {
                data.background = GetFileName('#home-background');
            }
            data.textColor = $('#home-text-color').val();
            data.mainText = $('#home-main-text').val();
            data.subTextList = subTextList;
            var response = AjaxCall("Admin", "SaveHome", data);
            if (response.Type == 'success') {
                Array.from(document.getElementsByClassName('home-remove-sub-text')).forEach(function (sub) {
                    sub.setAttribute('onclick', '');
                });
                document.getElementById('home-add-sub-text').setAttribute('onclick', '');
                DisableInputs('home-settings');
                if (!window.location.href.includes("UpdatePage")) {
                    NextSection();
                }
            }
            CreateDialog(response.Type, response.Type, response.Message, '', '', '');
        }
    } else {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    }
}

function SaveAndDisableAboutSection() {
    if (($('#about-section-image').val() == '' && !window.location.href.includes("UpdatePage")) || $('#about-header').val() == ''
        || $('#about-body').val() == '') {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    } else {
        var data = {};
        if (window.location.href.includes("UpdatePage")) {
            if ($('#about-section-image').val() != '') {
                data.image = GetFileName('#about-section-image');
            } else {
                data.image = null;
            }
        } else {
            data.image = GetFileName('#about-section-image');
        }
        data.background = $('#about-background-color').val();
        data.frame = $('#about-frame-color').val();
        data.header = $('#about-header').val();
        data.body = $('#about-body').val();
        data.subTitle = $('#about-sub-title').val();
        data.informationList = GetAboutInfo()[1];
        var flag = GetAboutInfo()[0];
        if (flag) {
            CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
        }
        else {
            var response = AjaxCall("Admin", "SaveAbout", data);
            if (response.Type == 'success') {
                Array.from(document.getElementsByClassName('about-info-remove')).forEach(function (element) {
                    element.setAttribute('onclick', '');
                });
                DisableInputs('about-settings');
                if (!window.location.href.includes("UpdatePage")) {
                    NextSection();
                }
            }
            CreateDialog(response.Type, response.Type, response.Message, '', '', '');
        }   
    }
}

function SaveAndDisableResumeSection() {
    var data = {};
    data.resumeSections = GetResumeItems();
    data.background = $('#resume-background-color').val();
    data.header = $('#resume-header').val();

    var response = AjaxCall("Admin", "SaveResume", data);
    if (response.Type == 'success') {
        Array.from(document.getElementsByClassName('resume-list-item')).forEach(function (element) {
            element.setAttribute('onclick', '');
        });
        DisableInputs('resume-settings');
        if (!window.location.href.includes("UpdatePage")) {
            NextSection();
        }
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function SaveAndDisablePortfolioSection() {
    if ($('#portfolio-header').val() == '') {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    } else {
        var categories = GetPortfolioCategories();
        if (categories[0]) {
            CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
        } else {
            var data = {};
            data.background = $('#portfolio-background-color-code').val();
            data.header = $('#portfolio-header').val();
            data.portfolioCategories = categories[1];
            var response = AjaxCall("Admin", "SavePortfolio", data);
            if (response.Type == 'success') {
                Array.from(document.getElementsByClassName('catagory-disable')).forEach(function (element) {
                    element.setAttribute('onclick', '');
                });
                DisableInputs('portfolio-settings');
                if (!window.location.href.includes("UpdatePage")) {
                    NextSection();
                }
            }
            CreateDialog(response.Type, response.Type, response.Message, '', '', '');
        }
    }
}

function SaveAndDisableBlogSection() {
    if ($('#blog-header').val() == '') {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    } else {
        var stories = GetBlogStories();
        if (stories[0]) {
            CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
        } else {
            var data = {};
            data.header = $('#blog-header').val();
            data.background = $('#blog-background-color').val();
            data.stories = stories[1];
            var response = AjaxCall("Admin", "SaveBlog", data);
            if (response.Type =='success') {
                Array.from(document.getElementsByClassName('disable-blog')).forEach(function (element) {
                    element.setAttribute('onclick', '');
                });
                DisableInputs('blog-settings');
                if (!window.location.href.includes("UpdatePage")) {
                    NextSection();
                }
            }
            CreateDialog(response.Type, response.Type, response.Message, '', '', '');
        }
    }
}

function SaveAndDisableContactSection() {
    if ($('#contact-header').val() != '') {
        var data = {};
        data.header = $('#contact-header').val();
        data.background = $('#contact-background-color').val();
        data.address = $('#street').val();
        data.city = $('#city').val();
        data.state = $('#state').val();
        data.country = $('#country').val();
        data.phone = $('#phone').val();
        data.email = $('#email').val();
        var response = AjaxCall("Admin", "SaveContact", data);
        if (response.Type == 'success') {
            DisableInputs('contact-settings');
            if (!window.location.href.includes("UpdatePage")) {
                NextSection();
            }
        }
        CreateDialog(response.Type, response.Type, response.Message, '', '', '');
    } else {
        CreateDialog('error', 'Error', 'Section header required!', '', '', '');
    }

}

function DisableInputs(id) {
    var inputDisable = "#" + id + " :input";
    Array.from($(inputDisable)).forEach(function (input) {
        input.disabled = true;
    });
}

function NextSection() {
    if (window.sectionQueue.length != 0) {
        var id = window.sectionQueue[0][2] + '-settings';
        document.getElementById(id).style.display = 'block';
        window.sectionQueue.shift();
    } else {
        document.getElementById('submit').style.display = 'block';
    }
    window.scrollTo(0, document.body.scrollHeight);
}

function GetSectionInformation() {
    var tableBody = document.getElementsByClassName('table-body')[0].children;
    let sectionInformationList = [];
    Array.from(tableBody).forEach(function (row) {
        let newSection = [];
        newSection.push(row.children[0].innerHTML);
        newSection.push(row.children[1].innerHTML);
        newSection.push(row.children[1].id);
        sectionInformationList.push(newSection);
    });
    return sectionInformationList;
}

Array.from(document.getElementsByClassName('toggle')).forEach(function (element) {
    element.addEventListener('click', function (event) {
        event.preventDefault();
        let targetID = event.target.id
        if ($(this).hasClass('active')) {
            document.getElementsByClassName(targetID)[0].style.display = 'none';
            event.target.parentElement.style.paddingBottom = '0px';
            event.target.parentElement.style.paddingTop = '0px';
            $(this).removeClass('active');
            $(this).next()
                .stop()
                .slideUp(300);

        } else {
            document.getElementsByClassName(targetID)[0].style.display = 'block';
            event.target.parentElement.style.paddingBottom = '25px';
            event.target.parentElement.style.paddingTop = '25px';
            $(this).addClass('active');
            $(this).next()
                .stop()
                .slideDown(300);
        }
    });
});

Array.from(document.getElementsByClassName('color-change')).forEach(function (element) {
    document.getElementById(element.id).addEventListener('change', function () {
        document.getElementById(element.id + '-code').value = document.getElementById(element.id).value;
    });
    document.getElementById(element.id + '-code').addEventListener('change', function () {
        document.getElementById(element.id).value = document.getElementById(element.id + '-code').value;
    });
});

Array.from(document.getElementsByClassName('custom-file-input')).forEach(function (element) {
    element.addEventListener('change', function () {
        var fileName = $(this).val();
        if (!(fileName.includes('.jpeg') || fileName.includes('.png') || fileName.includes('.jpg'))) {
            CreateDialog('warning', 'Upload Image Warning', 'You can only upload .jpg, .jpeg or .png type files!', '', '', '')
        } else {
            $(this).next('.custom-file-label').html(fileName.replace(/C:\\fakepath\\/i, ''));
        }
    });
});

function DeleteRow() {
    let deletedValue = window.delete.children[0].innerHTML;
    var tableElements = window.delete.parentElement;
    var flag = false;
    Array.from(tableElements.children).forEach(function (tr) {
        if (tr.children[0].innerHTML == deletedValue) {
            flag = true;
        }
        if (flag) {
            tr.children[0].innerHTML = Number.parseInt(tr.children[0].innerHTML) - 1;
        }
    });
    window.delete.remove();
}

function TableEventListener() {
    Array.from(document.getElementsByClassName('edit-row')).forEach(function (element) {
        element.addEventListener('click', function (event) {
            event.preventDefault();
            let row = this.parentElement.parentElement.parentElement;
            if (row.className != 'table-body') {
                $('#old-section-name').val(row.children[1].innerHTML);
                window.change = row;
            } else {
                $("table").delegate("tr", "click", function () {
                    $('#old-section-name').val(this.children[1].innerHTML);
                    window.change = this;
                });
            }
        });
    });

    Array.from(document.getElementsByClassName('delete-row')).forEach(function (element) {
        element.addEventListener('click', function (event) {
            event.preventDefault();
            window.delete = this.parentElement.parentElement.parentElement;
            if (window.delete.className != 'table-body') {
                if (window.location.href.includes("UpdatePage")) {
                    CreateDialog('warning', 'Delete Section', 'Do you want to delete this section?', 'Yes', 'DeleteNavigationSection()', '');
                } else {
                    CreateDialog('warning', 'Delete Section', 'Do you want to delete this section?', 'Yes', 'DeleteRow()', '');
                }
            }
            else {
                $("table").delegate("tr", "click", function () {
                    window.delete = this;
                });
            }
            if (window.location.href.includes("UpdatePage")) {
                CreateDialog('warning', 'Delete Section', 'Do you want to delete this section?', 'Yes', 'DeleteNavigationSection()', '');
            } else {
                CreateDialog('warning', 'Delete Section', 'Do you want to delete this section?', 'Yes', 'DeleteRow()', '');
            }
        });
    });
}

function ChangeSectionName() {
    if ($('#new-section-name').val() == "" || $('#new-section-name').val().trim() == "") {
        CreateDialog('error','Invalid Parameter',"You can't give empty name.",'','','');
    } else {
        let row = window.change;
        row.children[1].innerHTML = FirstLetterUpperCase($('#new-section-name').val());
        $('#new-section-name').val("");
        window.change = null;
    }
}

function FirstLetterUpperCase(string) {
    let flag = false;
    let newString = "";
    for (var index = 0; index < string.length; index++) {
        if (index == 0) {
            newString = newString + string[index].toUpperCase();
        }
        else if (flag) {
            flag = false;
            newString = newString + string[index].toUpperCase();
        }
        else if (string[index] == " ") {
            flag = true;
            newString = newString + string[index];
        }
        else {
            newString = newString + string[index].toLowerCase();
        }
    }
    return newString;
}

function AddSubTextToHome() {
    var innerHTML = '<div class="col-lg-6 new-item" >\
                        <div class="custom-file">\
                            <input type="text" class="form-control sub-text" placeholder="developer">\
                            <a href="javascript:void(0);" onclick="RemoveSubTextFromHome()" class="red close home-remove-sub-text" aria-label="Close" style="display:flex;">\
                                <span aria-hidden="true">&times;</span>\
                            </a>\
                        </div>\
                    </div>';
    document.getElementById('home-sub-text').appendChild(StringToHTML(innerHTML,' col-lg-12 input-padding')); 
}

function RemoveSubTextFromHome() {
    if (window.location.href.includes("UpdatePage")) {
        var data = {};
        data.subText = event.target.parentElement.parentElement.children[0].value;
        var response = AjaxCall("Admin", "DeleteSubTextFromHome", data);
        if (response.Type == 'success') {
            event.target.parentElement.parentElement.parentElement.parentElement.remove();
        }
        CreateDialog(response.Type, response.Type, response.Message, '', '', '');
    } else {
        event.target.parentElement.parentElement.parentElement.parentElement.remove();
    }
}

function DeleteSubText() {
    window.value = event.target.parentElement.parentElement.children[0].value;
    window.target = event.target.parentElement.parentElement.parentElement.parentElement;
    CreateDialog('warning', 'Warning', 'Do you want to delete this sub text?', 'Yes','DeleteSubTextFromHome()','')
}

function DeleteSubTextFromHome() {
    var data = {};
    data.subText = window.value;
    var response = AjaxCall("Admin", "DeleteSubTextFromHome", data);
    if (response.Type == "success") {
        window.target.remove();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function AddImageToPortfolio() {
    var innerHTML = '<div class="col-lg-6 new-item" >\
                        <div class="custom-file">\
                            <div class="custom-file" style="width:90%;">\
                                <input type="file" class="custom-file-input">\
                                <label class="custom-file-label">Choose file</label>\
                            </div>\
                            <a href="javascript:void(0);" onclick="RemoveImageFromPortfolio()" class="red close catagory-disable" aria-label="Close" style="display:flex;">\
                                <span aria-hidden="true">&times;</span>\
                            </a>\
                        </div>\
                      </div>';
    event.target.parentElement.parentElement.parentElement.appendChild(StringToHTML(innerHTML, ' col-lg-12 input-padding'));
    FileEventListenerRefresh();
}

function RemoveImageFromPortfolio() {
    var value = event.target.parentElement.parentElement.children[0].children[0].value;
    var label = event.target.parentElement.parentElement.children[0].children[1].innerHTML;
    window.target = event.target.parentElement.parentElement.parentElement.parentElement;

    if (label == 'Choose file' || value.includes('fakepath')) {
        window.target.remove()
    } else {
        CreateDialog('warning', 'Delete Image', 'Do you want to delete this image?', 'Yes', 'DeleteImageFromPortfolio()', '');
    }
}

function DeleteImageFromPortfolio() {
    window.target.remove()
}

function AddCatagory() {
    var innerHTML = '<div class="col-lg-12" style="padding-top:10px">\
                        <button type="button" onclick="RemoveCategory()" class="btn btn-danger" style = "float:right;"> Remove Catagory</button>\
                    </div >\
                    <div class="col-lg-6 input-padding">\
                        <b>Category:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control" placeholder="Branding">\
                        </div>\
                    </div>\
                    <div class="col-lg-6 input-padding">\
                        <b>Image:</b>\
                        <a href="javascript:void(0);" onclick="AddImageToPortfolio()" class="green close catagory-disable" style="float:none;" aria-label="Close">\
                            <span aria-hidden="true">&plus;</span>\
                        </a>\
                        <div class="custom-file" style="width:90%;">\
                            <input type="file" class="custom-file-input" >\
                            <label class="custom-file-label">Choose file</label>\
                        </div>\
                    </div>';
    event.target.parentElement.parentElement.parentElement.appendChild(StringToHTML(innerHTML,'form-row text-left catagory'));     
    FileEventListenerRefresh();
}

function DeleteCategoryMessage() {
    window.target = event.target.parentElement.parentElement;
    CreateDialog('warning', 'Delete Category', 'Do you want to delete this category?', 'Yes','DeleteCategory()','');
}

function DeleteCategory() {
    var category = window.target.children[1].children[1].children[0].value;
    var data = {}
    GetPortfolioCategories()[1].forEach(function (element) {
        if (category == element.category) {
            data = element;
        }
    });
    var response = AjaxCall("Admin", "DeletePortfolioCategory", data)
    if (response.Type == 'success') {
        window.target.remove();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function RemoveCategory() {
    event.target.parentElement.parentElement.remove();
}

function AddStory() {
    var innerHTML = '<div class="col-lg-12">\
                        <div class="col-lg-12" style="text-align:right; padding:0px; padding-top:10px;">\
                            <a href="javascript:void(0);" onclick="RemoveStory()" class="btn btn-danger disable-blog">Remove</a>\
                        </div>\
                    </div>\
                    <div class="col-lg-6 input-padding">\
                        <b>Title:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control" placeholder="Story Title">\
                        </div>\
                    </div>\
                    <div class="col-lg-6 input-padding">\
                        <b>Image:</b>\
                        <div class="custom-file">\
                            <input type="file" class="custom-file-input">\
                            <label class="custom-file-label">Choose file</label>\
                        </div>\
                    </div>\
                    <div class="col-lg-12 input-padding">\
                        <b>Body: </b>\
                        <textarea class="form-control" rows="3"></textarea>\
                    </div>';
    event.target.parentElement.parentElement.parentElement.parentElement.appendChild(StringToHTML(innerHTML, 'form-row text-left'));
    FileEventListenerRefresh();
}

function DeleteStoryMessage() {
    window.target = event.target.parentElement.parentElement.parentElement;
    CreateDialog('warning', 'Delete Story', 'Do you want to delete this stroy?', 'Yes', 'DeleteStory()', '');
}

function DeleteStory() {
    var data = {}
    data.title = window.target.children[1].children[1].children[0].value;
    data.image = window.target.children[2].children[1].children[1].innerHTML;
    data.body = window.target.children[3].children[1].value;
    var response = AjaxCall("Admin", "DeleteStory", data)
    if (response.Type == 'success') {
        window.target.remove();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function RemoveStory() {
    event.target.parentElement.parentElement.parentElement.remove();
}

function StringToHTML(str, cls) {
    var dom = document.createElement('div');
    dom.setAttribute('class', cls);
    dom.innerHTML = str;
    return dom;
};

function FileEventListenerRefresh() {
    Array.from(document.getElementsByClassName('custom-file-input')).forEach(function (element) {
        element.addEventListener('change', function () {
            var fileName = $(this).val();
            if (!(fileName.includes('.jpeg') || fileName.includes('.png') || fileName.includes('.jpg'))) {
                CreateDialog('warning', 'Upload Image Warning', 'You can only upload .jpg, .jpeg or .png type files!', '', '', '')
            } else {
                $(this).next('.custom-file-label').html(fileName.replace(/C:\\fakepath\\/i, ''));
            }
        });
    });
}

function NavigationDefaultSections(template) {
    let table = document.getElementsByClassName('table')[0];
    var tbody = document.createElement('tbody');
    tbody.setAttribute('class', 'table-body');
    var innerHTML = '';

    if (template == 'Folio') {
        var sectionList = ['Home','About','Portfolio','Blog','Contact'];
    }
    else if (template == 'MyResume' || template == 'iPortfolio') {
        var sectionList = ['Home', 'About', 'Resume', 'Portfolio', 'Contact'];
    }
    var counter = 1;
    const deleteButton = '<button class="table-icon delete-row close" href="javascript:void(0)" > <span class="fa fa-trash"></span></button>';
    const editButton = '<button class="table-icon edit-row close" href="javascript:void(0)" data-toggle="modal" data-target="#change-section-name" > <span class="fa fa-edit"></span></button>';
    let button = editButton;
    sectionList.forEach(function (row) {
        var tr = document.createElement('tr');
        if (counter == 2) {
            button = deleteButton + editButton;
        }
        innerHTML ='<th scope="row">' + counter + '</th>\
                    <td class="text-center" id="'+ row.toLowerCase()+ '">' + row + '</td>\
                    <td class="text-right">'+ button + '</td>';
        tr.innerHTML = innerHTML;
        tbody.appendChild(tr);
        counter++;
    });
    table.appendChild(tbody);
    TableEventListener();
}

//TODO
function Logout(){
    window.location.href = "../Admin/Logout";
}

function AddInformationToAboutSection() {
    var innerHTML = '<div class="col-lg-6 input-padding">\
                        <b> Information Title: </b>\
                        <a href="javascript:void(0);" onclick="RemoveInformation()" class="red close about-info-remove" aria-label="Close" style="position: absolute">\
                            <span aria-hidden="true">&times;</span>\
                        </a>\
                        <input type="text" class="form-control" placeholder="Age">\
                     </div>\
                     <div class="col-lg-6 input-padding">\
                        <b>Information Value: </b>\
                        <input type="text" class="form-control" placeholder="30">\
                     </div>';
    document.getElementById('about-extra-info').appendChild(StringToHTML(innerHTML,'add-info'));
}

function DeleteAboutInformationMessage() {
    window.target = event.target.parentElement.parentElement.parentElement;
    window.title = event.target.parentElement.parentElement.parentElement.children[0].children[2].value;
    window.value = event.target.parentElement.parentElement.parentElement.children[1].children[1].value;
    CreateDialog('warning', 'Delete Information', 'Do you want to delete this information?', 'Yes', 'DeleteAboutInformation()', '');
}

function DeleteAboutInformation() {
    var data = {};
    data.value = window.value;
    data.title = window.title;
    var response = AjaxCall("Admin", "DeleteInformation", data);
    if (response.Type == "success") {
        window.target.remove();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function RemoveInformation() {
    event.target.parentElement.parentElement.parentElement.remove();
}

function AddExplanation() {
    var innerHTML = '<div class="col-lg-12">\
                        <button type="button" class="btn btn-danger" style = "float:right; color: white; margin: 15px 0px 15px 15px;" onclick = "RemoveResumeItem()" > Resume Item</button>\
                     </div>\
                    <textarea class="form-control resume-item" rows="3"></textarea>';
    event.target.parentElement.parentElement.appendChild(StringToHTML(innerHTML, ''))
}

function RemoveResumeItem() {
    event.target.parentElement.parentElement.remove();
}

function AddResumeSection() {
    var innerHTML = '<div class="col-lg-12" >\
                        <button type="button" class="btn btn-danger catagory-disable" style="float:right; color: white" onclick="RemoveResumeSection()">Remove Resume Section</button>\
                     </div >\
                    <div class="col-lg-6 input-padding" >\
                        <b> Resume Section Header:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control resume-section-header" placeholder="Education">\
                        </div>\
                    </div>\
                    <div class="col-lg-12">\
                        <button type = "button" class="btn btn-success" style = "float:right; color: white" onclick = "AddResumeSubSection()" > Add Sub Section</button >\
                    </div>\
                    <div class="col-lg-12 input-padding">\
                        <b>Sub Header:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control resume-sub-header" placeholder="MASTER OF FINE ARTS & GRAPHIC DESIGN">\
                        </div>\
                    </div>\
                    <div class="col-lg-3 input-padding">\
                        <b>From:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control resume-date" placeholder="2001">\
                        </div>\
                    </div>\
                    <div class="col-lg-3 input-padding">\
                        <b>To:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control resume-date" placeholder="2005">\
                        </div>\
                    </div>\
                    <div class="col-lg-6 input-padding">\
                        <b>Place or Location:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control resume-location" placeholder="Rochester Institute of Technology, Rochester, NY">\
                         </div>\
                    </div>\
                    <div class="col-lg-12 input-padding">\
                        <b>Explanation: </b>\
                        <textarea class="form-control" rows="3"></textarea>\
                    </div>\
                    <div class="col-lg-12 input-padding">\
                        <b>List of Explanations: </b>\
                        <a href="javascript:void(0);" onclick="AddExplanation()" class="close resume-list-item" style="float:none;" aria-label="Close">\
                            <span class="green" aria-hidden="true">&plus;</span>\
                        </a>\
                        <textarea class="form-control resume-item" rows="3"></textarea>\
                    </div>';
    document.getElementById('resume-section').appendChild(StringToHTML(innerHTML, 'form-row text-left'));
}

function RemoveResumeSection() {
    event.target.parentElement.parentElement.remove();
}

function AddResumeSubSection() {
    var innerHTML = '<div class="col-lg-12">\
                        <button type = "button" class="btn btn-danger" style = "float:right; color: white" onclick = "RemoveSubSectionFromResume()" > Remove Sub Section</button >\
                     </div>\
                     <div class="col-lg-12 input-padding" >\
                        <b> Sub Header:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control resume-sub-header" placeholder="MASTER OF FINE ARTS & GRAPHIC DESIGN">\
                        </div>\
                     </div>\
                     <div class="col-lg-3 input-padding">\
                         <b>From:</b>\
                         <div class="custom-file">\
                             <input type="text" class="form-control resume-date" placeholder="2001">\
                         </div>\
                     </div>\
                     <div class="col-lg-3 input-padding">\
                         <b>To:</b>\
                         <div class="custom-file">\
                             <input type="text" class="form-control resume-date" placeholder="2005">\
                         </div>\
                     </div>\
                     <div class="col-lg-6 input-padding">\
                        <b>Place or Location:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control resume-location" placeholder="Rochester Institute of Technology, Rochester, NY">\
                        </div>\
                     </div>\
                    <div class="col-lg-12 input-padding">\
                        <b>Explanation: </b>\
                        <textarea class="form-control resume-explanation" rows="3"></textarea>\
                    </div>\
                    <div class="col-lg-12 input-padding">\
                        <b>List of Explanations: </b>\
                        <a href="javascript:void(0);" onclick="AddExplanation()" class="close resume-list-item" style="float:none;" aria-label="Close">\
                            <span class="green" aria-hidden="true">&plus;</span>\
                        </a>\
                        <textarea class="form-control resume-item" rows="3"></textarea>\
                    </div>';
    event.target.parentElement.parentElement.appendChild(StringToHTML(innerHTML,'row'));
}

function RemoveSubSectionFromResume() {
    event.target.parentElement.parentElement.remove();
}

Array.from(document.getElementsByClassName('update-info')).forEach(function (button) {
    button.addEventListener('click', function (event) {
        if (this.id == 'update-phone') {
            window.updatePhone = $('#update-phone-element').val();
            window.updateEmail = $('#update-email-element').val();
            document.getElementById('new-phone').style.display = 'block';
            document.getElementById('phone-update').style.display = 'block';
            document.getElementById('new-email').style.display = 'none';
            document.getElementById('email-update').style.display = 'none';
        } else {
            window.updateEmail = $('#update-email-element').val();
            document.getElementById('new-phone').style.display = 'none';
            document.getElementById('phone-update').style.display = 'none';
            document.getElementById('new-email').style.display = 'block';
            document.getElementById('email-update').style.display = 'block';
        }
    })
});

function UpdatePhone() {

    if (window.updatePhone != $('#update-phone-element').val()) {
        var data = {};
        data.oldPhone = window.updatePhone;
        data.newPhone = $('#update-phone-element').val();
        data.email = window.updateEmail;
        var response = AjaxCall("Admin", "UpdatePhone", data);
        CreateDialog(response.Type, response.Type, response.Message, '', '', 'RefreshPage()');
    }
    window.update = null;
}

function UpdateEmail() {
    if (window.updateEmail != $('#update-email-element').val()) {
        var data = {};
        data.oldEmail = window.updateEmail;
        data.newEmail = $('#update-email-element').val();
        var response = AjaxCall("Admin", "UpdateEmail", data);
        CreateDialog(response.Type, response.Type, response.Message, '', '', 'RefreshPage()');
    }
    window.update = null;
}

function DisplayEmail(value){
    $('#update-email-element').val(value);
}

function DisplayPhone(value) {
    $('#update-phone-element').val(value);
}

function ResetPassword()
{
    let controller = 'Admin';
    if (window.location.href.includes("User")) {
        controller = 'User';
    }

    let oldPassword = $('#old-password').val();
    let newPassword = $('#password').val();
    let confirmPassword = $('#confirm-password').val();
    if (newPassword == confirmPassword) {
        if (document.getElementById('strong-password').style.display == 'block') {
            document.getElementById('reset-password-response').innerHTML = "Please pick valid password";
            document.getElementsByClassName('alert-danger')[0].style.display = 'block';
        }
        else {
            var data = {};
            data.oldPassword = oldPassword;
            data.newPassword = newPassword;
            data.email = $('#update-email-element').val();
            var response = AjaxCall(controller, "ResetPassword", data);
            if (response.Type == 'success') {
                $('#old-password').val('');
                $('#password').val('');
                $('#confirm-password').val('');
                document.getElementsByClassName('alert-danger')[0].style.display = 'none';
                if (controller == "User") {
                    CreateDialog(response.Type, response.Type, response.Message, '', '', 'BackLoginPage()');
                }
                else
                {
                    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
                }
            } else {
                document.getElementsByClassName('alert-danger')[0].style.display = 'block';
                console.log(response.Message);
                document.getElementById('reset-password-response').innerHTML = response.Message;
            }
        }
    } else {
        document.getElementById('reset-password-response').innerHTML = "Confirm password is different!";
        document.getElementsByClassName('alert-danger')[0].style.display = 'block';
    }
}

function BackLoginPage()
{
    let url = window.location.href;
    url = url.replace('Reset', 'Login');
    window.location.href = url;
}

function AjaxCall(controller, method, data) {
    var result = '';
    $.ajax({
        type: "POST",
        url: "/" + controller + "/" + method,
        data: JSON.stringify(data),
        contentType: "application/json",
        dataType: "json",
        async: false,
        success: function (response) {
            result = response;
        },
        error: function (response) {
            result = response;
        }
    });
    return result;
}

function AddSocialMedia() {
    var newInputs = '<div class="col-lg-6 input-padding">\
                         <b> Social Media:</b>\
                            <div class="input-group mb-3">\
                                <select class="form-control" style="overflow:scroll;">\
                                    '+ document.getElementById('select-social-media').innerHTML +'\
                                </select>\
                            </div>\
                        </div>\
                        <div class="col-lg-6 input-padding">\
                            <b>Link:</b>\
                            <div class="input-group mb-3">\
                                <input type="text" class="form-control">\
                                <a href="javascript:void(0);" onclick="RemoveSocialMedia()" class="red close remove-social-media" aria-label="Close" style="display:flex; padding:5px;">\<span aria-hidden="true">&times;</span></a></div>\
                        </div>';
    document.getElementById('add-social-media').appendChild(StringToHTML(newInputs, 'form-row text-left social-media'));
}

function RemoveSocialMedia() {
    if(event.target.parentElement.parentElement.parentElement.parentElement.classList.value != ''){
        event.target.parentElement.parentElement.parentElement.parentElement.remove();
    }
}

function SocialMediaList() {
    var socialMediaList = [];
    Array.from(document.getElementsByClassName('social-media')).forEach(function (socialMedia) {
        var socialMediaItem = {};
        Array.from(socialMedia.getElementsByTagName('select')).forEach(function (option) {
            socialMediaItem.socialMedia = option.value;
        });
        Array.from(socialMedia.getElementsByTagName('input')).forEach(function (input) {
            socialMediaItem.link = input.value;
        });
        socialMediaList.push(socialMediaItem);
    });
    return socialMediaList;
}

function GetAboutInfo() {
    var infoPairList = [];
    var info = [];
    var counter = 0;
    var flag = false;
    Array.from($("#about-extra-info :input")).forEach(function (input) {
        if (counter != 0 && counter % 2 == 0) {
            infoPairList.push(info);
            info = [];
        }
        if (input.value == '' && counter > 1) {
            flag = true;
        }
        info.push(input.value);
        counter++;
    });
    infoPairList.push(info);
    return [flag,infoPairList];
}

function GetPortfolioCategories() {
    var counter = 0;
    var catagories = [];
    var newCatagory = {};
    var flag = false;
    var emptyInputFlag = false;
    var imageList = [];
    Array.from($("#portfolio-settings :input")).forEach(function (input) {
        if (!(counter < 4)) {
            if (input.type == 'text' || input.type == 'button') {
                if (flag) {
                    catagories.push(newCatagory);
                    newCatagory = new Object;
                    flag = false;
                }
                if (input.value == '' && input.type != 'button') {
                    if (!window.location.href.includes("UpdatePage")) {
                        emptyInputFlag = true;
                    }
                }
                if (input.type != 'button') {
                    imageList = [];
                    if (window.location.href.includes("UpdatePage")) {
                        if (input.value != '') {
                            newCatagory.category = input.value;
                        } else {
                            newCatagory.category = null;
                        }
                    } else {
                        newCatagory.category = input.value;
                    }
                }
            } else {
                if (input.value == '') {
                    if (!window.location.href.includes("UpdatePage")) {
                        emptyInputFlag = true;
                    }
                }
                flag = true;
                if (window.location.href.includes("UpdatePage")) {
                    if (input.name != '') {
                        imageList.push(input.name);
                    } else {
                        imageList.push(GetFileNameWithValue(input.value));
                    }
                } else {
                    imageList.push(GetFileNameWithValue(input.value));
                }
                newCatagory.images = imageList;
            }
        }
        counter++;
    });
    return [emptyInputFlag, catagories]
}

function GetFileName(id) {
    var file = $(id).val().split("\\");
    return file[2];
}

function GetFileNameWithValue(value) {
    var file = value.split("\\");
    return file[2];
}

function GetBlogStories() {
    var counter = 0;
    var stories = [];
    var newStory = {};
    var flag = false;
    Array.from($("#blog-settings :input")).forEach(function (input) {
        if (!(counter < 3)) {
            if (input.type == 'textarea') {
                newStory.body = input.value;
            }
            if (counter != 3 && counter % 3 == 0) {
                stories.push(newStory);
                newStory = new Object;
            }
            if (input.value == '' && input.type != 'button') {
                if (!window.location.href.includes("UpdatePage")) {
                    flag = true;
                }
            }
            if (input.type == 'text') {
                newStory.title = input.value;
            }
            if (input.type == 'file') {
                if (window.location.href.includes("UpdatePage")) {
                    if (input.value != '') {
                        newStory.image = GetFileNameWithValue(input.value);
                    } else {
                        newStory.image = null;
                    }
                } else {
                    newStory.image = GetFileNameWithValue(input.value);
                }
            }
        }
        counter++;
    });
    return [flag, stories]
}

function GetResumeItems() {
    var resume = [];
    var subHeaderList = [];
    var explanationList = [];

    var resumeHeader = new Object();
    var resumeSubHeader = new Object();

    var headerFlag = false;
    var subHeaderFlag = false;

    var counter = 0;
    var inputs = $("#resume-section :input");

    Array.from(inputs).forEach(function (input) {
        if (input.className.includes('resume-section-header')) {
            if (headerFlag) {
                if (!jQuery.isEmptyObject(resumeSubHeader)) {
                    subHeaderList.push(resumeSubHeader);
                    resumeHeader.resumeSubSections = subHeaderList
                    resume.push(resumeHeader);
                    resumeSubHeader = new Object();;
                }
                resumeHeader = new Object();;
                subHeaderList = [];
            }
            resumeHeader.header = input.value;
            headerFlag = true;
        }
        else if (input.className.includes('resume-sub-header')) {
            if (subHeaderFlag) {
                if (!jQuery.isEmptyObject(resumeSubHeader)) {
                    subHeaderList.push(resumeSubHeader);
                    resumeHeader.resumeSubSections = subHeaderList
                    resumeSubHeader = new Object();;
                }
                resumeHeader.resumeSubSections = subHeaderList
                explanationList = [];
            }
            resumeSubHeader.header = input.value;
            subHeaderFlag = true;
        }
        else if (input.className.includes('resume-date')) {
            resumeSubHeader.date = input.value;
        }
        else if (input.className.includes('resume-location')) {
            resumeSubHeader.location = input.value;
        }
        else if (input.className.includes('resume-explanation')) {
            resumeSubHeader.explanation = input.value;
        }
        else if (input.className.includes('resume-item')) {
            explanationList.push(input.value);
            resumeSubHeader.explanationItems = explanationList;
        }
        if (counter == inputs.length - 1) {
            subHeaderList.push(resumeSubHeader);
            resumeHeader.resumeSubSections = subHeaderList
            resume.push(resumeHeader);
        }
        counter++;
    });
    return resume;
}

function DeletePage() {
    var response = AjaxCall("Admin", "DeletePage", {});
    CreateDialog(response.Type, response.Type, response.Message, '', '','');
}

function DeleteResumeSectionMessage() {
    window.target = event.target.parentElement.parentElement;
    window.section = event.target.parentElement.parentElement.children[0].children[1].children[0].value;
    CreateDialog('warning', 'Delete Resume Section', 'Do you want to delete this resume section?', 'Yes', 'DeleteResumeSection()', '');
}
function DeleteResumeSection() {
    var data = {};
    data.section = window.section;
    var response = AjaxCall("Admin", "DeleteResumeSection", data);
    if (response.Type == "success") {
        window.target.remove();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function DeleteResumeSectionExplanationMessage() {
    window.target = event.target.parentElement.parentElement;
    window.section = event.target.parentElement.parentElement.parentElement.parentElement.children[0].children[1].children[0].value;
    window.explanation = event.target.parentElement.parentElement.children[1].innerHTML;
    let length = Array.from(event.target.parentElement.parentElement.children[1].classList).length - 1;
    window.subHeader = Array.from(event.target.parentElement.parentElement.children[1].classList)[length];
    CreateDialog('warning', 'Delete Resume Section Explanation', 'Do you want to delete this resume section explanation?', 'Yes', 'DeleteResumeSectionExplanation()', '');
}
function DeleteResumeSectionExplanation() {
    var data = {};
    data.section = window.section;
    data.explanation = window.explanation;
    data.subHeader = window.subHeader;
    var response = AjaxCall("Admin", "DeleteResumeSectionExplanation", data);
    if (response.Type == "success") {
        window.target.remove();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function DeleteSocialMedia() {
    window.target = event.target.parentElement.parentElement.parentElement;
    window.remove = event.target.parentElement.parentElement.parentElement.parentElement;
    if (typeof window.target.parentElement.children[0].children[1].children[0].value != 'undefined') {
        CreateDialog('warning', 'Delete Social Media', 'Do you want to delete this social media?', 'Yes', 'DeleteSocialMediaLink()', '');
    }
}

function DeleteSocialMediaLink() {
    var data = new Object();
    var remove = new Object();
    var numberOfMedias = document.getElementsByClassName('social-media').length;

    data.socialMedia = window.target.parentElement.children[0].children[1].children[0].value;
    data.link = window.target.children[1].children[0].value;
    remove.socialMedia = data;
    var response = AjaxCall("Admin", "DeleteSocialMediaLink", remove);
    if (response.Type == "success") {
        if (numberOfMedias == 1) {
            window.remove.children[1].children[1].children[0].value = '';
        } else {
            window.remove.remove();
        }
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');
}

function DeleteNavigationSection() {
    var content = window.delete.children[1].id;
    var data = {};
    data.content = content;
    var response = AjaxCall("Admin", "DeleteSectionFromNavigation", data);
    if (response.Type == "success") {
        let deletedValue = window.delete.children[0].innerHTML;
        var tableElements = window.delete.parentElement;
        var flag = false;
        Array.from(tableElements.children).forEach(function (tr) {
            if (tr.children[0].innerHTML == deletedValue) {
                flag = true;
            }
            if (flag) {
                tr.children[0].innerHTML = Number.parseInt(tr.children[0].innerHTML) - 1;
            }
        });
        window.delete.remove();
        document.getElementById(content + '-settings').remove();
    }
    CreateDialog(response.Type, response.Type, response.Message, '', '', '');   
}