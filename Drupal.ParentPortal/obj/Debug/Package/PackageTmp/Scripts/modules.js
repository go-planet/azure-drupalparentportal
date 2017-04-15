var msdpp = window.msdpp || {};

msdpp.modules = (function () {
   
    var initSubmitEvents = function() {
        jQuery('#VolunteerSubmit').click(function () {
            if (!jQuery("#frmVolunteer").valid()) {
                return;
            }
            //jQuery("#VolunteerSubmit").button('Loading');
            jQuery('#VolunteerSubmit').prop('disabled', true);
            //validation isn't working. Modal fields exist outside the form element on the page plus jQuery("#form0").valid() and jQuery("#form0").validate() methods are not valid
            var firstname = jQuery("#VolunteerInfo_FirstName").val();
            var lastname = jQuery("#VolunteerInfo_LastName").val();
            var phone = jQuery("#VolunteerInfo_Phone").val();
            var email = jQuery("#VolunteerInfo_Email").val();
            var streetaddress = jQuery("#VolunteerInfo_StreetAddress").val();
            var city = jQuery("#VolunteerInfo_City").val();
            var state = jQuery("#VolunteerInfo_State").val();
            var zip = jQuery("#VolunteerInfo_ZIPCode").val();
            var eventid = jQuery('.calendar-modalid').text();
            //  var userid = jQuery("#LoggedInUser").val(); //remove once integrated with Drupal
            //  var clientid = jQuery("#ClientId").val();//remove once integrated with Drupal
            // var secret = jQuery("#Secret").val();//remove once integrated with Drupal
            // var json = { clientid: clientid, secret: secret, model: { SchoolEventId: eventid, UserId: userid, FirstName: firstname, LastName: lastname, Phone: phone, Email: email, StreetAddress: streetaddress, City: city, State: state, ZIPCode: zip } };
            var json = { SchoolEventId: eventid, FirstName: firstname, LastName: lastname, Phone: phone, Email: email, StreetAddress: streetaddress, City: city, State: state, ZIPCode: zip };
            var jsonString = JSON.stringify(json);
            jQuery.ajax({
                type: "POST",
                url: '/modules/addvolunteer',
                data: { values: jsonString },
                accept: "text/html",
                success: function (result) {
                    if (result == null)
                    { console.log("Error submitting volunteer."); return; }
                    jQuery("#VolunteerForm").hide();
                    jQuery("#Volunteer").hide();
                    jQuery('.status').text("Thank you, " + firstname + ", for voluntering.").show();
                    jQuery('#volunteeredAlert').show();
                    jQuery("#UnvolunteerButton").parent().show();
                    jQuery('#calendar').fullCalendar('refetchEvents');
                },
                error: function (xhr, status, error) {
                    jQuery('.status').text(error);
                }
            }).then(function (result) {
                var r = result;
                jQuery('#VolunteerSubmit').prop('disabled', false);
            });

        });
        jQuery('#volunteerCheck').click(function () {
            if (jQuery("#volunteerCheck").prop("checked")) {
                jQuery("#VolunteerForm").show();
            } else {
                jQuery("#VolunteerForm").hide();
            }
        });
        jQuery('#CloseButton, .close').click(function () {
            msdpp.modules.resetVolunteerForm();
        });
        jQuery('#UnvolunteerButton').click(function () {
            var id = jQuery(this).data('id');
            msdpp.modules.cancelVolunteer(id);
            jQuery('.status').text("").hide();
            jQuery("#UnvolunteerButton").parent().hide();
            jQuery("#volunteerCheck").prop("checked", false);
            jQuery("#Volunteer").show();
            jQuery(".cancelVolunteer").filter('[data-id="' + id + '"]').parent().html("<div>Event removed.</div>");
        });
        jQuery('.cancelVolunteer').click(function () {
            var id = jQuery(this).data('id');
            msdpp.modules.cancelVolunteer(id);
            this.parentElement.innerHTML = "<div>Event removed.</div>";
        });
    }

    var initEventsCalendar = function() {

        jQuery('#calendar').fullCalendar({
            header: {
                left: 'prev,next,today',
                center: 'title',
                right: 'month,agendaWeek,agendaDay,listWeek'
            },
            footer: {
                left: '',
                center: '',
                right: ''
            },
            timezone: 'UTCS',
            //defaultDate: '2016-12-12',
            navLinks: true, // can click day/week names to navigate views
            editable: false,
            contentHeight: 400,
            eventLimit: true, // allow "more" link when too many events
            //events: JSON.parse(events),
            events: function (start, end, timezone, callback) {
                //jQuery('#calendarloader').show();
                msdpp.modules.loading("EventsModule");
                //var userid = "123";//jQuery("#LoggedInUser").val(); //remove once integrated with Drupal
                //var volunteerEventColor = "red"; //background and border color of events that users can volunteer for
                //var registeredVolunteerEventColor = "blue"; //background and border color of events that the logged in user has already registered for
                //var clientid = jQuery("#ClientId").val();//remove once integrated with Drupal
                //var secret = jQuery("#Secret").val();//remove once integrated with Drupal
                //var json = { clientid: clientid, secret: secret, userId: userid, start: start.toISOString(), end: end.toISOString(), userId: userid, volunteerEventColor: volunteerEventColor, registeredVolunteerEventColor: registeredVolunteerEventColor };
                var json = { start: start.toISOString(), end: end.toISOString() };
                jQuery.ajax({
                    type: "get",
                    data: json,
                    url: '/modules/getevents',
                    success: function (d) {
                        callback(d);
                        //jQuery('#calendarloader').hide();
                        msdpp.modules.finishedloading("EventsModule");
                    },
                    error: function (e) {
                        console.log(e);
                        //jQuery('#calendarloader').hide();
                        msdpp.modules.finishedloading("EventsModule");
                    }
                });
            },
            eventRender: function (event, element) {
                jQuery(element).tooltip({ title: "<b>" + event.title + "</b><br>Starts: " + event.start.format(" h:mm A, MMMM D YYYY") + "<br> Ends: " + event.end.format("h:mm A, MMMM D YYYY"), html: true, container: "body" });
                jQuery(element).click(function () {
                    msdpp.modules.resetVolunteerForm();
                    jQuery('.calendar-modaltitle').text(event.title);
                    jQuery('.calendar-modalid').text(event.schooleventid);
                    jQuery('.calendar-modalstart').text(event.start.format("MMMM DD YYYY, hh:mm A"));
                    var end = "";
                    if (event.end != null) end = event.end.format("MMMM DD YYYY, hh:mm A");
                    jQuery('.calendar-modalend').text(end);
                    jQuery('.calendar-modallocation').text(event.location == null ? "" : event.location);
                    jQuery('.calendar-modaldescription').text(event.description == null ? "" : event.description);
                    if (event.primarycontactsemail) {
                        jQuery('#emailContact').text(event.primarycontactsemail);
                        jQuery('#emailContact').attr("href", "mailto:" + event.primarycontactsemail);
                        jQuery('#emailContact').parent().parent().show();
                    }
                    jQuery('#UnvolunteerButton').data("id", event.schooleventid);
                    if (event.isvolunteeropportunity) {
                        //jQuery('#volunteerloader').show();
                        msdpp.modules.loading("VolunteerForm");
                        jQuery('#volunteeredAlert').hide();
                        //var userid = "123";//jQuery("#LoggedInUser").val(); //remove once integrated with Drupal
                        //var clientid = jQuery("#ClientId").val();//remove once integrated with Drupal
                        //var secret = jQuery("#Secret").val();//remove once integrated with Drupal
                        //var json = { clientid: clientid, secret: secret, userid: userid, eventid: event.schooleventid };
                        var json = { eventid: event.schooleventid };
                        jQuery.ajax({
                            type: "GET",
                            url: "/modules/isuservolunteer",
                            data: json,
                            accept: "text/html",
                            success: function (result) {
                                //jQuery('#volunteerloader').hide();
                                if (result) {
                                    jQuery('.status').text("Thank you for voluntering.").show();
                                    jQuery('#volunteeredAlert').show()
                                    if (event.end > Date.now()) {
                                        jQuery("#UnvolunteerButton").parent().show();
                                        jQuery("#Volunteer").hide();
                                    }
                                } else {
                                    if ((event.end > Date.now()) && (event.maxvolunteers == 0 || (event.registeredvolunteers < event.maxvolunteers))) {
                                        jQuery("#Volunteer").show();
                                        jQuery("#UnvolunteerButton").parent().hide();
                                    }
                                }
                                msdpp.modules.finishedloading("VolunteerForm");
                            },
                            error: function (result) {
                                //jQuery('#volunteerloader').hide();
                                msdpp.modules.finishedloading("VolunteerForm");
                            }
                        });

                    }
                    if ((event.actionlink) && (event.actionbuttontext)) {
                        jQuery("#ActionBtn").text(event.actionbuttontext);
                        jQuery("#ActionBtn").attr("href", event.actionlink);
                        jQuery("#ActionBtn").parent().parent().show();
                    }
                    jQuery('#eventDetailsModal').modal();
                });
                //element.qtip({
                //    content: event.title + "<br>Start: " + event.start.format("MMMM DD YYYY, hh:mm A") + "<br><b>" + event.end.format("MMMM DD YYYY, hh:mm A")
                //});
            }
        });

    }

    var resetVolunteerForm = function() {
        jQuery("#status").hide();
        jQuery('.status').text("");
        jQuery("#Volunteer").hide();
        jQuery("#UnvolunteerButton").parent().hide();
        jQuery("#volunteerCheck").prop("checked", false);
        jQuery("#VolunteerForm").hide();
        jQuery('#emailContact').text("");
        jQuery('#emailContact').attr("href", "");
        jQuery('#emailContact').parent().parent().hide();
        jQuery("#ActionBtn").text("");
        jQuery("#ActionBtn").attr("href", "");
        jQuery("#ActionBtn").parent().parent().hide();
        jQuery("#VolunteerSubmit").button('reset');
        jQuery('#volunteeredAlert').hide();
    }

    var cancelVolunteer = function(id) {
        jQuery('#volunteeredAlert').hide();
        var json = { eventid: id };
        jQuery.ajax({
            type: "POST",
            url: "/modules/unvolunteer",
            data: json,
            accept: "text/html",
            success: function (result) {
                console.log("Volunteer canceled.");
                jQuery("#VolunteerSubmit").button('reset');
                jQuery('#calendar').fullCalendar('refetchEvents');
            },
            error: function (xhr, status, error) {
                console.log("Error canceling. Error: " + error);
            }
        }).then(function (result) {

        });
    }

    var initMyStudents = function () {
        msdpp.modules.loading("MyStudentsModule");
        jQuery('#NewStudentbutton').click(function () {
            jQuery('#newStudentModal').modal('show');
            msdpp.modules.finishedloading("AddStudentForm");
        });
        jQuery('#newStudentModal').on('shown.bs.modal', function () {
            jQuery('#NewStudent_StudentsSchoolId').focus();
        });
        jQuery('.RemoveStudentButton').click(function () {
            var id = jQuery(this).data('id');
            var name = jQuery(this).data('name');
            msdpp.modules.confirmDelete(id, name);
            msdpp.modules.finishedloading("RemoveStudentConfirmation");
            //removeStudent(id, this);
            //this.parentElement.innerHTML = "<div>Event removed.</div>";
        });
        msdpp.modules.finishedloading("MyStudentsModule");
    }

    var removeStudent = function () {
        msdpp.modules.loading("RemoveStudentConfirmation");
        jQuery('#removeFailedAlert').hide();
        var id = jQuery('#proceedBtn').data('id');
        var json = { studentid: id };
        jQuery.ajax({
            type: "POST",
            url: "/modules/removestudent",
            data: json,
            accept: "application/json",
            success: function (result) {
                if (result.Success) {
                    //jQuery("#studentid" + result.StudentId).remove();
                    console.log("Student Removed.");
                    //jQuery('#confirm-delete').modal('hide');
                    //jQuery('#proceedBtn').data('id', "");        // TODO: we need to do a full reload to get the updated student info in other modules. show a message similar to save student and reload
                    window.location.reload();
                    //jQuery('#removeSucceededAlert').show();
                    //setTimeout(function () { window.location.reload(); }, 500);
                } else {

                    jQuery('#removeFailedAlert').show();
                    msdpp.modules.finishedloading("RemoveStudentConfirmation");
                    jQuery('#confirm-delete').modal('hide');
                    console.log("Error removing student.");
                }
            },
            error: function (xhr, status, error) {
                jQuery('#removeFailedAlert').show();
                msdpp.modules.finishedloading("RemoveStudentConfirmation");
                jQuery('#confirm-delete').modal('hide');
                console.log("Error removing student. Error: " + error);
            }
        }).then(function (result) {

        });
    }

    /* todo: remove?
    var saveStudentSucceeded = function(data) {
        if (data.Success) {
            jQuery("#saveSucceededAlert").show();
            jQuery("#saveSucceededAlert").alert();
            if (data.Content != "") {
                jQuery("#myStudentsTable").html(data.Content);
                msdpp.modules.initMyStudents();
            }
            jQuery('.modal-backdrop').hide();
            jQuery('#newStudentModal').modal('hide');
            jQuery("#parent-selector :input").attr("disabled", false);
            jQuery("#form0")[0].msdpp.modules.resetVolunteerForm();
            jQuery("#btnSave").button('reset');
            jQuery("#saveSucceededAlert").hide();
            jQuery("#form0").find('input, textarea, select').attr("disabled", false);
        } else {
            if (data.Message != "") {
                console.log(data.Message);
            }
            jQuery("#form0").find('input, textarea, select').attr("disabled", false);
            jQuery("#btnSave").button('reset');
            jQuery("#saveFailedAlert").show();
            jQuery("#saveFailedAlert").alert();
            jQuery("#saveFailedAlert").fadeTo(8000, 500).slideUp(500, function () {
                jQuery("#saveFailedAlert").slideUp(500);
            });
        }
    }

    function SaveStudentFailed(data) {
        console.log(data);
    }

        //function ConfirmUpdate() {
    //    jQuery('#confirm-update').modal('show');
    //}

     */

    var confirmDelete = function(id, name) {
        jQuery('#proceedBtn').data('id', id);
        jQuery('#removeStudentName').html(name);
        jQuery('#confirm-delete').modal('show');
    }

    var cancelDelete = function() {
        jQuery('#confirm-delete').modal('hide');
        jQuery('#proceedBtn').data('id', "");
    }

    var resizeStudentTable = function() {
        jQuery("#myStudentsTable table").each(function () {

            if (jQuery(this).width() < 480) {
                jQuery(this).find("td.rhd").addClass("rhd-hidden");
            }
            else {
                jQuery(this).find("td.rhd").removeClass("rhd-hidden");
            }

        });
    }

    var getDocumentCount = function() {

        jQuery("#document-pager-select").off().on("change", function () { msdpp.modules.getDocumentsByPage(); });

        msdpp.modules.loading("DocumentsModule");

        jQuery.ajax({
            type: "GET",
            url: "/modules/getdocumentcount",
            accept: "appliction/json",
            success: function (result) {
                //jQuery("#doclistloader").hide();
                if (result && result.Success) {
                    var count = parseInt(result.Message);
                    var pages = Math.floor(count / 10);
                    if (pages > 0) {
                        var elem = jQuery("#document-pager-select");

                        for (var i = 0; i < pages; i++) {
                            var val = parseInt(i) + 1;
                            var text = parseInt(i) + 2;
                            elem.append("<option value='" + val + "'>Page " + text + "</option>");
                        }

                        // Grab the documents for the current page
                        msdpp.modules.getDocumentsByPage();
                    }
                }
                msdpp.modules.finishedloading("DocumentsModule");
            },
            error: function (result) {
                //jQuery("#doclistloader").hide();
                msdpp.modules.finishedloading("DocumentsModule");
            }
        });
    }

    var getDocumentsByPage = function() {
        var val = jQuery("#document-pager-select").val();
        if (!val) return;
        var json = { page: val };

        //jQuery("#doclistloader").show();
        msdpp.modules.loading("DocumentsModule");

        jQuery.ajax({
            type: "GET",
            url: "/modules/getdocuments",
            data: json,
            accept: "application/json",
            success: function (result) {
                //jQuery("#doclistloader").hide();
                if (result && result.Success) {

                    var documents = result.Message;

                    html = [];

                    for (var i = 0; i < documents.length; i++) {
                        html.push('<div class="list-item">');
                        html.push('<div class="container">');
                        html.push('<div class="row">');
                        html.push('<div class="col-md-12 text-nowrap document-listing-row" style="padding-left: 0 !important;">');
                        html.push('<div class="document-docicon document-icon">' + documents[i].Extension + '</div>');
                        html.push('&nbsp;<a href="modules/download?documentid=' + documents[i].DocumentId + '&documentname=' + documents[i].Name + '" class="document-listing-href" target="_blank" title="' + documents[i].Name + '">' + documents[i].Name + '</a>');
                        html.push('</div></div></div></div>');

                    }

                    var htmlstring = html.join('');
                    jQuery("#document-listing").html(htmlstring);

                }
                msdpp.modules.finishedloading("DocumentsModule");
            },
            error: function (result) {
                //jQuery("#doclistloader").hide();
                msdpp.modules.finishedloading("DocumentsModule");
            }
        });
    }

    var initSubmitStudent = function () {
        jQuery('#btnSaveStudent').click(function () {
            if (!jQuery('#frmStudent').valid()) return;
            msdpp.modules.loading("AddStudentForm");
            /* TODO - what is this - if (!jQuery("#frmVolunteer").valid()) {
                  return;
              }*/
            // TODO - jQuery("#VolunteerSubmit").button('loading');
            //validation isn't working. Modal fields exist outside the form element on the page plus jQuery("#form0").valid() and jQuery("#form0").validate() methods are not valid

            jQuery('#saveFailedAlert').hide();
            jQuery('#saveSucceededAlert').hide();

            var studentid = jQuery("#NewStudent_StudentsSchoolId").val();
            var firstname = jQuery("#NewStudent_FirstName").val();
            var lastname = jQuery("#NewStudent_LastName").val();
            var school = jQuery("#NewStudent_School").val();
            var teacher = jQuery("#NewStudent_Teacher").val();
            var grade = jQuery("#NewStudent_GradeLevel").val();
            var dob = jQuery("#NewStudent_DoB").val();

            var json = { StudentsSchoolId: studentid, FirstName: firstname, LastName: lastname, School: school, Teacher: teacher, GradeLevel: grade, DoB: dob };
            var jsonString = JSON.stringify(json);
            jQuery('#btnCancelAddStudent').prop('disabled', true);
            jQuery('#btnSaveStudent').prop('disabled', true);
            jQuery.ajax({
                type: "POST",
                url: '/modules/savestudent',
                data: { values: jsonString },
                accept: "application/json",
                success: function (result) {
                    if (!result || !result.Success) {
                        jQuery('#saveFailedAlert').show();
                        jQuery('#btnCancelAddStudent').prop('disabled', false);
                        jQuery('#btnSaveStudent').prop('disabled', false);
                        msdpp.modules.finishedloading("AddStudentForm");
                    }
                    else {
                        jQuery('#saveSucceededAlert').show();
                        msdpp.modules.finishedloading("AddStudentForm");
                        window.location.reload();
                        //setTimeout(function () { window.location.reload(); }, 500);
                    }
                },
                error: function (xhr, status, error) {
                    jQuery('#saveFailedAlert').show();
                    jQuery('#btnCancelAddStudent').prop('disabled', false);
                    jQuery('#btnSaveStudent').prop('disabled', false);
                    msdpp.modules.finishedloading("AddStudentForm");
                }
            }).then(function (result) {
                var r = result;
            });

        });
    }

    var cancelAddStudent = function () {
        jQuery('#newStudentModal').modal('hide');
        document.getElementById("frmStudent").reset();
    }


    var initOfficeModule = function () {
        msdpp.modules.loading("OfficeModule");
        msdpp.modules.finishedloading("OfficeModule");
    }

    var loading = function(viewbagmodule) {
        jQuery('.loaderModal.' + viewbagmodule).show();
        //jQuery('.modal-backdrop').appendTo('.module-content.' + viewbagmodule);
        //console.log('Loading ' + viewbagmodule);
    }

    var finishedloading = function(viewbagmodule) {
        jQuery('.loaderModal.' + viewbagmodule).hide();
        //console.log('Finished loading ' + viewbagmodule);
    }

    return {
        initSubmitEvents: initSubmitEvents,
        initEventsCalendar: initEventsCalendar,
        resetVolunteerForm: resetVolunteerForm,
        cancelVolunteer: cancelVolunteer,
        initMyStudents: initMyStudents,
        cancelAddStudent: cancelAddStudent,
        removeStudent: removeStudent,
        confirmDelete: confirmDelete,
        cancelDelete: cancelDelete,
        resizeStudentTable: resizeStudentTable,
        getDocumentCount: getDocumentCount,
        getDocumentsByPage: getDocumentsByPage,
        initSubmitStudent: initSubmitStudent,
        initOfficeModule: initOfficeModule,
        loading: loading,
        finishedloading: finishedloading
    }
})();


jQuery(document).ready(function () {
    //jQuery.noConflict("true");
    //  msdpp.modules.loading("@ViewBag.Module"); // TODO
    msdpp.modules.getDocumentCount();
    msdpp.modules.initMyStudents();
    msdpp.modules.initSubmitStudent();
    msdpp.modules.initSubmitEvents();
    msdpp.modules.initEventsCalendar();
    msdpp.modules.resizeStudentTable();
    msdpp.modules.initOfficeModule();
    // msdpp.modules.finishedloading("@ViewBag.Module"); // TODO
});

jQuery(document).resize(function () {
    msdpp.modules.resizeStudentTable();
});
