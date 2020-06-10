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
        var response = AjaxCall("/User/Authorization", data);
        if (response != 'Success') {
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
    var response = AjaxCall("/User/ConfirmEmail", data);
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
    let seconds = 60;
    let timer;

    if (document.getElementById('email-checkbox').checked == true) {

    }

    if (document.getElementById('phone-checkbox').checked == true) {

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
        var response = AjaxCall("/User/CreateUser",data);        
        if (response == 'Success') {
            document.getElementsByClassName('alert-danger')[0].style.display = 'none';
            CreateDialog('success', 'Success', 'Your account has been created.', '', '','window.history.back()');
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

window.addEventListener('load', (event) => {
    if (window.location.href.includes("Register") || window.location.href.includes("Settings")) {
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
    if (window.location.href.includes("CreateBlog")) {
        document.getElementById('create-blog').onsubmit = function () {
            var formdata = new FormData();
            var fileInput = document.getElementById('fileInput');
            Array.from(document.getElementsByClassName('custom-file-input')).forEach(function (fileInput) {
                for (i = 0; i < fileInput.files.length; i++) {
                    formdata.append(fileInput.files[i].name, fileInput.files[i]);
                }
            });
            var xhr = new XMLHttpRequest();
            xhr.open('POST', '/Admin/Upload');
            xhr.send(formdata);
            xhr.onreadystatechange = function () {
                if (xhr.readyState == 4 && xhr.status == 200) {
                    //alert(xhr.responseText);
                } else {
                    //alert(xhr.responseText + "else");
                }
            }
        }    
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
                                <div class="modal-header" style="background:'+ color +'">\
                                    <h4 class="modal-title" id="myModalLabel">'+ title +'</h4>\
                                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">\
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
    var response = AjaxCall("/Admin/SaveTemplate", data);
    if (response.includes('saved') || response.includes('updated')) {
        document.getElementById('template-button').disabled = true;
        Array.from(templates).forEach(function (element) {
            element.onclick = '';
        });
        document.getElementById('main-settings').style.display = 'block';
        window.scrollTo(0, document.body.scrollHeight);
        SetDefaultValues();
        CreateDialog('success', 'Success', response, '', '', '');
    } else {
        CreateDialog('error', 'Error', response, '', '', '');
    }

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
}

function SaveAndDisableMainSection() {
    if ($('#web-page-logo').val() != '' && $('#web-page-title').val() != '') {
        var data = {};
        var logo = $('#web-page-logo').val().split("\\");
        data.logo = logo[2];
        data.title = $('#web-page-title').val();
        data.titleColor = $('#main-title-color').val();
        data.textColor = $('#main-text-color').val();
        data.hoverColor = $('#main-hover-color').val();
        data.socialMediaList = SocialMediaList();
        var response = AjaxCall("/Admin/SaveMainComponents", data);
        if (response.includes('saved')) {
            CreateDialog('success', 'Success', response, '', '', '');
            Array.from(document.getElementsByClassName('remove-social-media')).forEach(function (element) {
                element.setAttribute('onclick', '');
            });
            DisableInputs('main-settings');
            document.getElementById('nav-settings').style.display = 'block';
            window.scrollTo(0, document.body.scrollHeight);
        } else {
            CreateDialog('error', 'Error', response, '', '', '');
        }
    } else {
        if ($('#web-page-logo').val() == '') {
            CreateDialog('error', 'Required parameter!', 'Web Page Logo is required!', '', '', '');
        } else {
            CreateDialog('error', 'Required parameter!', 'Web Page Title is required!','','','');
        }
    }
}

function SaveAndDisableNavigationSection() {
    if ($('#navigation-logo').val() != '') {
        var data = {}
        data.barColor = $('#nav-bar-color').val();
        var logo = $('#navigation-logo').val().split("\\");
        data.logo = logo[2];
        var navigationItems = [];
        GetSectionInformation().forEach(function (element) {
            var item = {};
            item.priority = element[0];
            item.sectionName = element[1];
            item.content = element[2];
            navigationItems.push(item);
        });
        data.navigationItems = navigationItems;
        var response = AjaxCall("/Admin/SaveNavigation", data);
        if (response.includes('saved')) {
            CreateDialog('success', 'Success', response, '', '', '');
            window.sectionQueue = GetSectionInformation();
            DisableInputs('nav-settings');
            NextSection();
        } else {
            CreateDialog('error', 'Error', response, '', '', '');
        }

    } else {
        CreateDialog('error', 'Required parameter!', 'Logo is required!', '', '', '');
    }
  
}

function SaveAndDisableHomeSection() {
    if ($('#home-background').val() != '' || $('#home-main-text').val() != '') {
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
            var logo = $('#home-background').val().split("\\");
            data.background = logo[2];
            data.textColor = $('#home-text-color').val();
            data.mainText = $('#home-main-text').val();
            data.subTextList = subTextList;
            var response = AjaxCall("/Admin/SaveHome", data);
            if (response.includes('saved.')) {
                CreateDialog('success', 'Success', response, '', '', '');
                Array.from(document.getElementsByClassName('home-remove-sub-text')).forEach(function (sub) {
                    sub.setAttribute('onclick', '');
                });
                document.getElementById('home-add-sub-text').setAttribute('onclick', '');
                DisableInputs('home-settings');
                NextSection();
            } else {
                CreateDialog('error', 'Error', response, '', '', '');
            }
        }
    } else {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    }
}

function SaveAndDisableAboutSection() {
    if ($('#about-section-image').val() == '' || $('#about-header').val() == ''
        || $('#about-body').val() == '') {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    } else {
        var data = {};
        var image = $('#about-section-image').val().split("\\");
        data.image = image[2];
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
            var response = AjaxCall("/Admin/SaveAbout", data);
            if (response.includes('saved.')) {
                CreateDialog('success', 'Success', response, '', '', '');
                Array.from(document.getElementsByClassName('about-info-remove')).forEach(function (element) {
                    element.setAttribute('onclick', '');
                });
                DisableInputs('about-settings');
                NextSection();
            } else {
                CreateDialog('error', 'Error', response, '', '', '');
            }
        }   
    }
}

function SaveAndDisableResumeSection() {
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
                    resumeHeader.subHeaders = subHeaderList
                    resume.push(resumeHeader);
                    resumeSubHeader = new Object();;
                }
                resumeHeader = new Object();;
                subHeaderList = [];
            }
            resumeHeader.title = input.value;
            headerFlag = true;
        }
        else if (input.className.includes('resume-sub-header')) {
            if (subHeaderFlag) {
                if (!jQuery.isEmptyObject(resumeSubHeader)) {
                    subHeaderList.push(resumeSubHeader);
                    resumeHeader.subHeaders = subHeaderList
                    resumeSubHeader = new Object();;
                }
                resumeHeader.subHeaders = subHeaderList
                explanationList = [];
            }
            resumeSubHeader.title = input.value;
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
            resumeSubHeader.items = explanationList;
        }
        if (counter == inputs.length - 1) {
            subHeaderList.push(resumeSubHeader);
            resumeHeader.subHeaders = subHeaderList
            resume.push(resumeHeader);
        }
        counter++;
    });

    window.resume = resume;
    window.resumeBackground = $('#resume-background-color').val();
    window.resumeHeader = $('#resume-header').val();

    Array.from(document.getElementsByClassName('resume-list-item')).forEach(function (element) {
        element.setAttribute('onclick', '');
    });

    DisableInputs('resume-settings');
    NextSection();
}

function SaveAndDisablePortfolioSection() {
    if ($('#portfolio-header').val() == '') {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    } else {
        var counter = 0;
        var catagories = [];
        var newCatagory = [];
        var flag = false;
        var emptyInputFlag = false;
        Array.from($("#portfolio-settings :input")).forEach(function (input) {
            if (!(counter < 4)) {
                if (input.type == 'text' || input.type == 'button') {
                    if (flag) {
                        catagories.push(newCatagory);
                        newCatagory = [];
                        flag = false;
                    }
                    if (input.value == '' && input.type != 'button') {
                        emptyInputFlag = true;
                    }
                    newCatagory.push(input.value)
                } else {
                    if (input.value == '') {
                        emptyInputFlag = true;
                    }
                    flag = true;
                    newCatagory.push(input.value);
                }
            }
            counter++;
        });
        if (emptyInputFlag) {
            CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
        } else {
            Array.from(document.getElementsByClassName('catagory-disable')).forEach(function (element) {
                element.setAttribute('onclick', '');
            });
            window.portfolioBackgroundColor = $('#portfolio-background-color-code').val();
            window.portfolioHeader = $('#portfolio-header').val();
            window.portfolioCatagories = catagories;
            DisableInputs('portfolio-settings');
            NextSection();
        }
    }
}

function SaveAndDisableBlogSection() {
    if ($('#blog-header').val() == '') {
        CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
    } else {
        var counter = 0;
        var stories = [];
        var newStory = [];
        var flag = false;
        Array.from($("#blog-settings :input")).forEach(function (input) {
            if (!(counter < 3)) {
                if (counter != 3 && counter % 3 == 0) {
                    stories.push(newStory);
                    newStory = [];
                }
                if (input.value == '' && input.type != 'button') {
                    flag = true;
                }
                newStory.push(input.value);
            }
            counter++;
        });
        if (flag) {
            CreateDialog('error', 'Invalid Input', 'Fill all empty fields!', '', '', '');
        } else {
            window.blogHeader = $('#blog-header').val();
            window.blogBackground = $('#blog-background-color').val();
            window.blogStories = stories;
            Array.from(document.getElementsByClassName('disable-blog')).forEach(function (element) {
                element.setAttribute('onclick', '');
            });
            DisableInputs('blog-settings');
            NextSection();
        }
    }
}

function SaveAndDisableContactSection() {
    window.contactHeader = $('#contact-header').val();
    window.contactBackground = $('#contact-background-color').val();
    window.contactStreet = $('#street').val();
    window.contactCity = $('#city').val();
    window.contactState = $('#state').val();
    window.contactCountry = $('#country').val();
    window.contactPhone = $('#phone').val();
    window.contactEmail = $('#email').val();
    DisableInputs('contact-settings');
    NextSection();
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
                CreateDialog('warning', 'Delete Section', 'Do you want to delete this section?', 'Yes', 'DeleteRow()', '');
            }
            else {
                $("table").delegate("tr", "click", function () {
                    window.delete = this;
                });
            }
            CreateDialog('warning', 'Delete Section', 'Do you want to delete this section?', 'Yes', 'DeleteRow()', '');
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
    event.target.parentElement.parentElement.parentElement.parentElement.remove();
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
    event.target.parentElement.parentElement.parentElement.parentElement.remove();
}

function AddCatagory() {
    var innerHTML = '<div class="col-lg-12">\
                        <button type="button" onclick="RemoveCatagory()" class="btn btn-danger" style = "float:right;"> Remove Catagory</a>\
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

function RemoveCatagory() {
    event.target.parentElement.parentElement.remove();
}

function AddStory() {
    var innerHTML = '<div class="col-lg-12">\
                        <div class="col-lg-12" style="text-align:right; padding:0px;">\
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
    event.target.parentElement.parentElement.parentElement.appendChild(StringToHTML(innerHTML, 'form-row text-left'));
    FileEventListenerRefresh();
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

function CreateBlog() {
    var blog = {};
    blog.template = window.template;

    blog.mainLogo = window.mainLogo;
    blog.mainTitle = window.mainTitle;
    blog.mainTitleColor = window.mainTitleColor;
    blog.mainTextColor = window.mainTextColor;
    blog.mainHoverColor = window.mainHoverColor;

    blog.navColor = window.navColor;
    blog.navLogo = window.navLogo;
    blog.sectionList = window.sectionList;

    
    blog.homeTextColor = window.homeTextColor;
    blog.homebackground = window.homebackground;
    blog.homeMainText = window.homeMainText;
    blog.homeSubText = window.homeSubText;

    blog.aboutBackgroundColor = window.aboutBackgroundColor;
    blog.aboutFrameColor = window.aboutFrameColor;
    blog.aboutImage = window.aboutImage;
    blog.aboutHeader = window.aboutHeader;
    blog.aboutBody = window.aboutBody;
    blog.aboutSubTitle = window.aboutSubTitle;
    blog.aboutExtraInfo = window.aboutExtraInfo;

    blog.portfolioBackgroundColor = window.portfolioBackgroundColor;
    blog.portfolioHeader = window.portfolioHeader;
    blog.portfolioCatagories = window.portfolioCatagories;

    blog.blogBackground = window.blogBackground;
    blog.blogHeader = window.blogHeader;
    blog.blogStories = window.blogStories;

    blog.contactHeader = window.contactHeader;
    blog.contactStreet = window.contactStreet;
    blog.contactCity = window.contactCity;
    blog.contactState = window.contactState;
    blog.contactCountry = window.contactCountry;
    blog.contactEmail = window.contactEmail;
    blog.contactPhone = window.contactPhone;
    blog.contactBackground = window.contactBackground

    blog.resume = window.resume;
    blog.resumeBackground = window.resumeBackground;
    blog.resumeHeader = window.resumeHeader;

    console.log(blog);
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
        var response = AjaxCall("/Admin/UpdatePhone", data);

        if (response != 'Success') {
            CreateDialog('error', 'Error', response, '', '', '');
        }
        else {
            CreateDialog('success', 'Success', "Phone number updated successfully.", '', '', 'RefreshPage()');
        }
    }
    window.update = null;
}

function UpdateEmail() {
    if (window.updateEmail != $('#update-email-element').val()) {
        var data = {};
        data.oldEmail = window.updateEmail;
        data.newEmail = $('#update-email-element').val();
        var response = AjaxCall("/Admin/UpdateEmail", data);

        if (response != 'Success') {
            CreateDialog('error', 'Error', response, '', '', '');
        }
        else {
            CreateDialog('success', 'Success', "Email updated successfully.", '', '', 'RefreshPage()');
        }
    }
    window.update = null;
}

function DisplayEmail(value){
    $('#update-email-element').val(value);
    console.log(value);
}

function DisplayPhone(value) {
    $('#update-phone-element').val(value);
}

function ResetPassword() {
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
            var response = AjaxCall("/Admin/ResetPassword",data);

            if (response == 'Success') {
                $('#old-password').val('');
                $('#password').val('');
                $('#confirm-password').val('');
                document.getElementsByClassName('alert-danger')[0].style.display = 'none';
                CreateDialog('success', 'Success', "Password changed successfully.", '', '', '');
            } else {
                document.getElementsByClassName('alert-danger')[0].style.display = 'block';
                document.getElementById('reset-password-response').innerHTML = response;
            }
        }
    } else {
        document.getElementById('reset-password-response').innerHTML = "Confirm password is different!";
        document.getElementsByClassName('alert-danger')[0].style.display = 'block';
    }
}

function AjaxCall(url, data) {
    var result = '';
    $.ajax({
        type: "POST",
        url: url,
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
    event.target.parentElement.parentElement.parentElement.parentElement.remove();
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
    if (flag) {
        infoPairList.push(info);
    }
    return [flag,infoPairList];
}