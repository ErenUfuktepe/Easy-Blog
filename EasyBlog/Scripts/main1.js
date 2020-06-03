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
        var result = null;
        $.ajax({
            type: "POST",
            url: "/User/Authorization",
            data: JSON.stringify(data),
            contentType: "application/json",
            dataType: "json",
            async: false,
            success: function (response) {
                if (response == "Success") {
                    result = response;
                    let url = '';
                    if (window.location.href.includes("Login") || window.location.href.includes("Authorization")) {
                        url = window.location.href;
                        url = url.replace('Authorization', 'Entry');
                        url = url.replace('Login', 'Entry');
                        window.location.href = url;
                    } else {
                        window.location.href = "User/Entry";
                    }
                } else {
                    result = response;
                }
            },
            error: function (response) {
                result = response;
            }
        });
        if (result != 'Success') {
            document.getElementById('login-response').innerHTML = result;
            document.getElementsByClassName('alert-danger')[0].style.display = 'block';
        }
    }
}

function ConfirmEmail() {
    let data = {};
    data.email = $('#email-confirm').val();
    var result = null;
    $.ajax({
        type: "POST",
        url: "/User/ConfirmEmail",
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

    var parsedResult = result.split(",");

    if (parsedResult.length ==  1) {
        document.getElementById('confirm-email-response').innerHTML = result;
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
        var result = null;
        $.ajax({
            type: "POST",
            url: "/User/CreateUser",
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
        if (result == 'Success') {
            document.getElementsByClassName('alert-danger')[0].style.display = 'none';
            CreateDialog('success', 'Success', 'Your account has been created.', '', '','window.history.back()');
        }
        else {
            document.getElementById('register-response').innerHTML = result;
            document.getElementsByClassName('alert-danger')[0].style.display = 'block';
        }
    } else {
        document.getElementById('register-response').innerHTML = "Invalid parameter";
        document.getElementsByClassName('alert-danger')[0].style.display = 'block';
    }
}

window.addEventListener('load', (event) => {
    if (window.location.href.includes("Register")) {
        document.getElementById('password').addEventListener('click', function () {
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
    document.getElementById('template-button').disabled = true;

    var templates = document.getElementsByClassName('image-padding');
    var selected = document.getElementsByClassName('active-image')[0];

    let url = selected.id;
    var array = url.split("/");
    window.template = array[array.length - 2];
    Array.from(templates).forEach(function (element) {
        element.onclick = '';
    });
    document.getElementById('main-settings').style.display = 'block';
    SetDefaultValues();
}

function SetDefaultValues() {
    if (window.template == 'Folio') {
        Folio();
    }
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
    window.mainHoverColor = $('#web-page-logo').val();
    window.mainHoverColor = $('#web-page-title').val();
    window.mainTitleColor = $('#main-title-color').val();
    window.mainTextColor = $('#main-text-color').val();
    window.mainHoverColor = $('#main-hover-color').val();
    DisableInputs('main-settings');
    document.getElementById('nav-settings').style.display = 'block';
}

function SaveAndDisableNavigationSection() {
    window.navColor = $('#nav-bar-color').val();
    window.navLogo = $('#navigation-logo').val();
    window.sectionList = GetSectionInformation();
    window.sectionQueue = window.sectionList;
    DisableInputs('nav-settings');
    NextSection();
}

function SaveAndDisableHomeSection() {
    window.homeTextColor = $('#home-text-color').val();
    window.homebackground = $('#home-background').val();
    window.homebackground = $('#home-main-text').val();
    DisableInputs('home-settings');

    var subTextList = [];
    Array.from(document.getElementsByClassName('sub-text')).forEach(function (sub) {
        subTextList.push(sub.value);
    });
    window.homeSubText = subTextList;
    Array.from(document.getElementsByClassName('home-remove-sub-text')).forEach(function (sub) {
        sub.setAttribute('onclick', '');
    });

    document.getElementById('home-add-sub-text').setAttribute('onclick', '');
    NextSection();
}

function SaveAndDisableAboutSection() {
    window.aboutBackgroundColor = $('#about-background-color').val();
    window.aboutFrameColor = $('#about-frame-color').val();
    window.aboutImage = $('#about-section-image').val();
    window.aboutHeader = $('#about-header').val();
    window.aboutBody = $('#about-body').val();
    DisableInputs('about-settings');
    NextSection();
}

function SaveAndDisablePortfolioSection() {
    window.portfolioBackgroundColor = $('#portfolio-background-color-code').val();
    window.portfolioHeader = $('#portfolio-header').val();

    DisableInputs('portfolio-settings');
    NextSection();
}

function SaveAndDisableBlogSection() {

    DisableInputs('blog-settings');
    NextSection();
}

function SaveAndDisableContactSection() {

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
        console.log('finish');
    }
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
        $(this).next('.custom-file-label').html(fileName.replace(/C:\\fakepath\\/i, ''));
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
                            <a href="javascript:void(0);" onclick="RemoveImageFromPortfolio()" class="red close" aria-label="Close" style="display:flex;">\
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
                        <a href="javascript:void(0);" onclick="RemoveCatagory()" class="btn btn-danger" style = "float:right; color:white;"> Remove Catagory</a>\
                    </div >\
                    <div class="col-lg-6 input-padding">\
                        <b>Category:</b>\
                        <div class="custom-file">\
                            <input type="text" class="form-control" placeholder="Branding">\
                        </div>\
                    </div>\
                    <div class="col-lg-6 input-padding">\
                        <b>Image:</b>\
                        <a href="javascript:void(0);" onclick="AddImageToPortfolio()" class="green close" style="float:none;" aria-label="Close">\
                            <span aria-hidden="true">&plus;</span>\
                        </a>\
                        <div class="custom-file" style="width:90%;">\
                            <input type="file" class="custom-file-input" >\
                            <label class="custom-file-label">Choose file</label>\
                        </div>\
                    </div>';
    event.target.parentElement.parentElement.parentElement.appendChild(StringToHTML(innerHTML,'form-row text-left'));     
    FileEventListenerRefresh();
}

function RemoveCatagory() {
    event.target.parentElement.parentElement.remove();
}

function AddStory() {
    var innerHTML = '<div class="col-lg-12">\
                        <div class="col-lg-12" style="text-align:right; padding:0px;">\
                            <a href="javascript:void(0);" onclick="RemoveStory()" class="btn btn-danger">Remove</a>\
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
            $(this).next('.custom-file-label').html(fileName.replace(/C:\\fakepath\\/i, ''));
        });
    });
}

//TODO
function Logout(){
    window.location.href = "../Admin/Logout";
}

function UpdateInformation() {

}