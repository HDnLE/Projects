var category = $("#category").data("category");
_timer(0);
var timeout;
$(".ts-input").focus();
$(".ts-input").select();

function _timer(x) {
    var timer = 60*60;
    if (x == 1) {
        clearTimeout(timeout);
    } else {
        timeout = setInterval(function () {
            if (timer == 60) {$("#modalSessionExpire").modal("show");}
			if (timer == 0) {window.location.href = "logout_v2.php"; }
			$("#session").html("Your session is about to expire due to inactivity. You will be logged out in <b>" + timer + "</b> seconds. Click <b>Stay Signed In</b> to continue your session.");
			//$("#liSessionTimer").html("Time before logout: " + secondsToMS(timer));
			$("#divTimeout").html("<span class='bold' style='font-size:14px;color:#EEE'>Session expires in:</span>" + secondsToMS(timer));
			timer--;
        }, 1000);
    }
}

function secondsToMS(s) 
{    
    var m = Math.floor(s/60);
    s -= m*60;
    return (m < 10 ? '0'+m : m)+":"+(s < 10 ? '0'+s : s);
}

$('#btnResetTimer').click(function(){
    _timer(1);
	$("#modalSessionExpire").modal("hide");
	_timer(0);
});

$('#btnSignOut').click(function(){
    window.location.href = "logout.php";
});

$("#liSessionTimer").hide();

// Disable Ctrl+B (Add Bookmark shortcut in Opera browser)
$(document).keydown(function (e) 
{
    var kc = e.which || e.keyCode;
	if (e.ctrlKey && String.fromCharCode(kc).toUpperCase() == "B")
	{
		e.preventDefault();
	}
});

$(document).on( "keypress", "body", function(e)
{
	if (e.keyCode == 10) 
	{
		$("#modalSessionTimeout2").modal("show");
	}
});
setInterval(function () 
{
	var currentdate = new Date(); 
	var cDay = currentdate.getDay();
	var cDate = (currentdate.getDate() < 10) ? "0" + currentdate.getDate() : currentdate.getDate();
	//var cMonth = ((currentdate.getMonth()+1) < 10) ? "0" + (currentdate.getMonth()+1) : (currentdate.getMonth()+1);
	var cMonth = getMonthName((currentdate.getMonth()+1));
	var cHours = (currentdate.getHours() < 10) ? "0" + currentdate.getHours() : currentdate.getHours();
	var cMinutes = (currentdate.getMinutes() < 10) ? "0" + currentdate.getMinutes() : currentdate.getMinutes();
	var cSeconds = (currentdate.getSeconds() < 10) ? "0" + currentdate.getSeconds() : currentdate.getSeconds();
	var datetime = getStringDay(cDay) + " " + cDate + ". "
					+ cMonth  + " " 
					+ currentdate.getFullYear() + " kl. "  
					+ cHours + ":"  
					+ cMinutes;
	$("#liDateTime").html(datetime);
}, 1000);

function getStringDay(intDay)
{
	var days = ["Søndag", "Mandag", "Tirsdag", "Onsdag", "Torsdag", "Fredag", "Lørdag"];
	return days[intDay];
} 

function getMonthName(intMonth)
{
	var monthName;
	switch (intMonth)
	{
		case 1:
			monthName = "januar";
			break;
		case 2:
			monthName = "febuar";
			break;
		case 3:
			monthName = "mars";
			break;
		case 4:
			monthName = "april";
			break;
		case 5:
			monthName = "mai";
			break;
		case 6:
			monthName = "juni";
			break;
		case 7:
			monthName = "juli";
			break;
		case 8:
			monthName = "august";
			break;
		case 9:
			monthName = "september";
			break;
		case 10:
			monthName = "oktober";
			break;
		case 11:
			monthName = "november";
			break;
		case 12:
			monthName = "desember";
			break;
	}
	return monthName;
}

jQuery(function($)
{
	$(document).ready(function()
	{
		
		/////////////////////////////////
		
		
		/////////////////////////////////
		
		localStorage.setItem('name', window.location.href);
		
		//************* Navigating to a page *****************//
		$(document).on( "click", "#btnLogout", function()
		{
			window.location.href = "logout.php";
		});
		//********************************************//
		
		//************* Navigating to a page *****************//
		$(document).on( "click", ".btn-pagination", function()
		{
			var page = $(this).data("page");
			var limit = $("#limit").val();
			var current_url = location.href;
			if (getUrlParam()["page"] != undefined) {current_url = current_url.replace("page=" + getUrlParam()["page"], "page=" + page);}
			else { current_url = current_url + "&page=" + page + "&limit=" + limit;}
			window.location.href = current_url;
		});
		//********************************************//
		
		//************* Jumping to a page *****************//
		$(document).on( "change", "#page", function()
		{
			var page = $(this).val();
			var current_url = location.href;
			if (getUrlParam()["page"] != undefined) {current_url = current_url.replace("page=" + getUrlParam()["page"], "page=" + page);}
			else { current_url = current_url + "&page=" + page + "&limit=25";}
			window.location.href = current_url;
		});
		//********************************************//
		
		//************* Changing number of rows per page *****************//
		$(document).on( "change", "#limit", function()
		{
			var limit = ($(this).val() == 999999) ? "Alle" : $(this).val();
			var current_url = location.href;
			if (getUrlParam()["limit"] != undefined) {current_url = current_url.replace("page=" + getUrlParam()["page"] + "&limit=" + getUrlParam()["limit"], "page=1&limit=" + limit);}
			else { current_url = current_url + "&page=1" + "&limit=" + limit;}
			window.location.href = current_url;
		});
		//********************************************//
		
		//************* Add new item *****************//
		$(document).on( "click", "#mnuBtnNew", function(e)
		{
			var mCategory = "";
			$(".modal").find("input:not([readonly])").val("");
			$(".modal").find("select").prop('selectedIndex',0);
			switch (category)
			{
				case "testsuite":
					$("#ts_id").val("Auto");
					mCategory = "testsuite";
					break;
				case "testcase_all":
				case "testcase_single":
					$("#tc_id").val("Auto");
					$("#btnActivate").hide();
					mCategory = "testcase";
					var ts_id = getUrlParam()["ts-id"];
					$('.form-control option[value=' + ts_id + ']').prop('selected',true);
					$("#infoMsg").hide();
					category = "testcase";
					break;
				case "teststeps":
					mCategory = "teststeg";
					$(".modal").find('#btnSave').attr("data-add-action", "addNew");
					var currentStep = $(this).data("step-number");
					$(".modal").find("#stepNum").val(currentStep);
					break;
				case "parent_objects":
					$("#txtParentID").val("Auto");
					mCategory = "objekt";
					break;
				case "child_objects":
					mCategory = "child objekt";
					break;
				case "test_commands":
					mCategory = "test kommando";
					$(".modal").find("#btnActivate").hide();
					break;
				case "updates":
					mCategory = "endringer/oppdateringer";
					break;
				case "tasks":
					mCategory = "oppgave";
					break;
				case "users":
					$(".modal").find("#btnToggleActivateUser, #btnResetPassword").hide();
					$(".modal").find(".user-form").show();
					break;
			}
			$(".modal").find('#btnSave').attr("data-action", "reg_" + category);
			//$(".modal").find('#modalTitle').text("Registrere ny " + mCategory);
			$(".modal").find("#btnDelete").hide();
		});
		//********************************************//
		
		//*********** Delete button from menu bar ***************//
		$(document).on( "click", "#mnuBtnDelete", function()
		{
			var action = "";
			$(".modal-loading").modal("hide");
			switch (category)
			{
				case "testsuite":
					action = "ts_delete";
					break;
				case "testcase_all":
				case "testcase_single":
					action = "tc_delete";
					break;
				case "teststeps":
					action = "step_delete";
					break;
				case "parent_objects":
					action = "parent_objects_delete";
					break;
				case "child_objects":
					action = "child_objects_delete";
					break;
				case "test_commands":
					action = "test_commands_delete";
					break;
				case "updates":
					action = "update_delete";
					break;
				case "tasks":
					action = "task_delete";
					break;
			}
			if ($('.cbox-default:checked').length == 0) 
			{
				$("#errModal").modal("show");
				$(".modal").find('#modalTitle').html("<span class='mdi mdi-alert text-warning icon-md'></span> Informasjon");
				showInfoMessage("danger", "<b>Feil:</b> Ingen elementer er valgt", false, "#infoModal");
				return false;
			}
			$("#modalDelete").modal("show");
			$(".modal").find("#btnConfirmDelete").attr("data-button-action", action);
		});
		//********************************************//
		
		//*********** Reload table list ***************//
		$(document).on( "click", "#mnuBtnRefresh", function()
		{
			showWaitModal(200);
		});
		//********************************************//
		
		//*********** Edit item in a row ***************//
		$(document).on( "click", ".edit-class", function()
		{
			var id = $(this).data("id");
			var mCategory = "";
			var modalTitle = "";
			var showModalTitle = true;
			switch (category)
			{
				case "testsuite":
					var title = $(this).data("title");
					var description = $(this).data("description");
					var tsID = $(this).data("ts-id");
					var active = $(this).data("active");
					var testGroup = $(this).data("group");
					var btnText = (active) ? "Deaktiver" : "Aktiver";
					$("#ts_id").val(tsID);
					$("#title").val(title);
					$("#description").val(description);
					$("#cboTestGroup").val(testGroup);
					mCategory = "testsuite";
					$(".modal").find("#btnDeactivateTS").show();
					$(".modal").find("#btnDeactivateTS").attr("data-ts-id", tsID);
					$(".modal").find("#btnDeactivateTS").text(btnText);
					break;
				case "testcase_all":
				case "testcase_single":
					var title = $(this).data("title");
					var comments = $(this).data("comments");
					var tcID = $(this).data("tc-id");
					var tsID = $(this).data("ts-id");
					var active = $(this).data("active");
					var ticket = $(this).data("ticket");
					var btnText = (active) ? "Deaktiver" : "Aktiver";
					var disabled = (!active) ? $("#infoMsg").show() : $("#infoMsg").hide();
					$("#btnActivate").text(btnText);
					$("#btnActivate").attr("data-active", active);
					$('.form-control option[value=' + tsID + ']').prop('selected',true);
					$(".modal").find("#btnActivate").show();
					$(".modal").find("#tc_id").val(tcID);
					$(".modal").find("#title").val(title);
					$(".modal").find("#comments").val(comments);
					$(".modal").find("#rec_id").val(id);
					$(".modal").find("#txtJira").val(ticket);
					$(".modal").find("#btnActivate").attr("data-action", "activate_testcase");
					$(".modal").find("#btnActivate").attr("data-id", id);
					mCategory = "testcase";
					category = "testcase";
					modalTitle = " (TC-" + tcID + ")";
					break;
				case "teststeps":
					mCategory = "teststeg";
					var stepNumber = $(this).data("step-num");
					var stepCategory = $(this).data("category");
					var stepSubCategory = 	$(this).data("sub-category");
					var testData = $(this).data("test-data");
					var description = $(this).data("description");
					var autoLogin = $(this).data("auto-login");
					$("#stepNum").val(stepNumber);
					$("#cboCategory").val(stepCategory);
					$(".modal").find("#txtCommand").val(testData.replace(/<br>/g, ""));
					$("#txtDescription").val(description);
					$("#chkAutoLogin").prop("checked", autoLogin);
					ajaxFormSubmit("get_subcategory", "&category=" + stepCategory + "&sub-category=" + stepSubCategory, false);
					break;
				case "parent_objects":
					mCategory = "parent objekt";
					var parentId = $(this).data("parent-id");
					var controlName = $(this).data("control-name");
					var objectName = $(this).data("object-name");
					var rxPath = $(this).data("rx-path");
					var description = $(this).data("description");
					$("#txtParentID").val(parentId);
					$("#txtControlName").val(controlName);
					$("#txtWinTitle").val(objectName);
					$("#txtRxPath").val(rxPath);
					$(".modal").find("#txtDescription").val(description);
					id = parentId;
					break;
				case "child_objects":
					mCategory = "child objekt";
					var objectType = $(this).data("object-type");
					var controlName = $(this).data("control-name");
					var rxPath = $(this).data("rx-path");
					var description = $(this).data("description");
					var childID = $(this).data("child-id");
					//$("#cboChildType option:contains(" + objectType + ")").prop("selected", true);
					$("#cboChildType option").each(function () 
					{
						if ($(this).html() == objectType) {
							$(this).attr("selected", "selected");
							return;
						}
					});
					$(".modal").find("#txtControlName").val(controlName);
					$(".modal").find("#txtRxPath").val(rxPath);
					$(".modal").find("#txtDescription").val(description);
					$(".modal").find("#btnSave").attr("data-child-id", childID);
					if (objectType == "Form")
					{
						category = "parent_objects";
						var parentId = $(this).data("parent-id");
						var parentName = $(this).data("parent-name");
						$("#txtParentID").val(parentId);
						$("#txtWinTitle").val(parentName);
					}
						
					break;
				case "test_commands":
					mCategory = "test kommando";
					if ($(this).data("target") == "#addminErrModal") showModalTitle = false;
					var cmdID = $(this).data("cmd-id");
					var cmdCategory = $(this).data("category");
					var command = $(this).data("format");
					var format = $(this).data("command");
					var cmdDescription = $(this).data("description");
					var cmdActive = $(this).data("active");
					var btnText = (cmdActive) ? "Deaktiver" : "Aktiver";
					$(".modal").find("#txtCmdID").val(cmdID);
					$(".modal").find("#cboCmdCategory").val(cmdCategory);
					$(".modal").find("#txtFormat").val(format);
					$(".modal").find("#txtCommand").val(command);
					$(".modal").find("#txtDescription").val(cmdDescription);
					$(".modal").find("#btnActivate").show();
					$(".modal").find("#btnActivate").text(btnText);
					$(".modal").find("#btnActivate").attr("data-active", cmdActive);
					$(".modal").find("#btnActivate").attr("data-action", "activate_command");
					$(".modal").find("#btnActivate").attr("data-id", cmdID);
					break;
				case "updates":
					mCategory = "endringer/oppdateringer";
					if ($(this).data("target") == "#addminErrModal") showModalTitle = false;
					var logDate = $(this).data("date");
					var strUpdate = $(this).data("update");
					$(".modal").find("#txtLogDate").val(logDate);
					$(".modal").find("#txtUpdates").val(strUpdate);
					$(".modal").find("#btnSave").attr("data-record-id", id);
					break;
				case "tasks":
					mCategory = "oppgave";
					var taskDate = $(this).data("task-date");
					var strTask = $(this).data("task");
					var status = $(this).data("status");
					var taskArea = $(this).data("task-area");
					$(".modal").find("#txtTaskDate").val(taskDate);
					$(".modal").find("#cboTaskArea").val(taskArea);
					$(".modal").find("#txtTask").val(strTask);
					$(".modal").find("#cboStatus").val(status);
					$(".modal").find("#btnSave").attr("data-record-id", id);
					break;
				case "roles":
					$("#frmAddRoleForm").trigger("reset");
					$(".modal").find("#txtRoleName").val($(this).data("role"));
					$(".modal").find("#btnSaveRole, #btnDeleteRole").attr("data-record-id", id);
					$("#btnDeleteRole").show();
					var permissions = $(this).data("permission");
					var arrPermissions = permissions.split("|");
					var chkboxIds = ["#chkTSAdd", "#chkTSView", "#chkTSEdit", "#chkTSDelete", "#chkTCAdd", "#chkTCView", "#chkTCEdit", "#chkTCDelete", "#chkCmdAdd", "#chkCmdView", "#chkCmdEdit", "#chkCmdDelete", "#chkObjAdd", "#chkObjView", "#chkObjEdit", "#chkObjDelete", "#chkTaskAdd", "#chkTaskView", "#chkTaskEdit", "#chkTaskDelete", "#chkTCManager", "#chkStepAdd", "#chkStepView", "#chkStepEdit", "#chkStepDelete"];
					for (x=0; x< chkboxIds.length; x++)
					{
						var isCheck = (arrPermissions[x] == 1)
						$(".modal").find(chkboxIds[x]).prop("checked", isCheck);
					}
					break;
				case "users":
					var btnActive = $(this).data("active");
					var username = $(this).data("username");
					var role = $(this).data("role");
					var btnText = (btnActive) ? "Deactivate" : "Activate";
					$(".modal").find("#btnToggleActivateUser").text(btnText);
					$(".modal").find("#btnToggleActivateUser, #btnSaveUser, #btnResetPassword").attr("data-username", username);
					$(".modal").find("#btnToggleActivateUser, #btnResetPassword").show();
					$(".modal").find(".user-form").hide();
					$(".modal").find("#txtPassword").val("");
					$('.form-control option[value=' + role + ']').prop('selected',true);
					break;
			}
			$(".modal").find("#btnDelete").attr("data-record-id", id);
			//if (showModalTitle) $(".modal").find('#modalTitle').text("Endre " + mCategory + modalTitle);
			$(".modal").find('#btnSave').attr("data-action", "edit_" + category);
			$(".modal").find("#btnDelete").show();
		});
		//********************************************//
		
		//*********** Save a new/modified item ***************//
		$(document).on( "click", "#btnSave", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			var action = $(this).data("action");
			var form_id = $(this).data("form");
			var serialize_form = $(form_id).serialize();
			$(".modal").modal("hide");
			if (action == "reg_teststeps" || action == "edit_teststeps")
			{
				var ts_id = getUrlParam()["ts-id"];
				var tc_id = getUrlParam()["tc-id"];
				serialize_form = serialize_form + "&ts-id=" + ts_id + "&tc-id=" + tc_id;
				if (action == "reg_teststeps")
				{
					var add_action = $(this).data("add-action");
					serialize_form = serialize_form + "&ts-id=" + ts_id + "&tc-id=" + tc_id + "&add-action=" + add_action;
				}
			}
			else if (action == "reg_child_objects" || action == "edit_child_objects")
			{
				var parentID = getUrlParam()["parent-id"];
				var childType = $("#cboChildType option:selected").text();
				serialize_form = serialize_form + "&parent-id=" + parentID + "&child-type=" + childType;
				if (action == "edit_child_objects")
				{
					var childID = $(this).data("child-id");
					serialize_form = serialize_form + "&child-id=" + childID;
				}
			}
			else if (action == "edit_updates" || action == "edit_tasks")
			{
				var id = $(this).data("record-id");
				serialize_form = serialize_form + "&id=" + id;
			}
			ajaxFormSubmit(action, serialize_form);
			
			showWaitModal(500);
		});
		//********************************************//
		
		//*********** Delete button from modal ***************//
		$(document).on( "click", "#btnDelete", function()
		{
			var action = $(this).data("button-action");
			var response = confirm("Slette denne registreringen kan ikke fortrykkes. Fortsette?");
			var id = $(this).data("record-id");
			if (response)
			{
				$(".modal").modal("hide");
				ajaxFormSubmit(action, "&id=" + id, false);
				showWaitModal(1500);
			}
		});
		//********************************************//
		
		//*********** Confirm delete button ***************//
		$(document).on( "click", "#btnConfirmDelete", function()
		{
			var action = $(this).data("button-action");
			$(".cbox-default:checked").each(function () 
			{
				var id = $(this).val();
				if (category == "parent_objects") id = $(this).data("parent-id");
				ajaxFormSubmit(action, "&id=" + id, false);
			});
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Copy button from menu bar ***************//
		$(document).on( "click", "#mnuBtnCopy", function()
		{
			if ($('.cbox-default:checked').length == 0) 
			{
				$("#errModal").modal("show");
				$(".modal").find(".modal-title").text("Informasjon");
				showInfoMessage("danger", "<b>Feil:</b> Ingen element er valgt", false, "#infoModal");
				return false;
			}
			var divModal = "#modalCopy";
			switch (category)
			{
				case "testsuite":
					action = "copy_testsuite";
					mCategory = "testsuite";
					break;
				case "testcase_all":
				case "testcase_single":
					action = "copy_testcase";
					var ts_id = getUrlParam()["ts-id"];
					$('.form-control option[value=' + ts_id + ']').prop('selected',true);
					mCategory = "testcase";
					break;
				case "teststeps":
					divModal = "#modalStepCopyMove";
					action = "copy_teststep";
					$("#rbToBottom").prop("checked", true);
					$("#txtStepNum").prop("disabled", true);
					$("#txtStepNum").val("");
					mCategory = "teststeg";
					var lblText1 = $("#lblCustom").text();
					var lblText2 = $("#lblTop").text();
					var lblText3 = $("#lblBottom").text();
					$(".modal").find(".modal-title").text("Kopiere teststeg");
					$("#lblCustom").text(lblText1.replace("%label%", "Kopi"));
					$("#lblTop").text(lblText2.replace("%label%", "Kopi"));
					$("#lblBottom").text(lblText3.replace("%label%", "Kopi"));
					break;
			}
			$(divModal).modal("show");
			$(".modal").find('#modalTitle').text("Kopiere " + mCategory);
			$(".modal").find("#btnConfirmCopy").attr("data-button-action", action);
		});
		//********************************************//
		
		$(document).on( "change", "#chkAction", function()
		{
			$('.cbox-default').not(":disabled").prop("checked", $(this).prop("checked"));
			$('.cbox-default').closest('tr').removeClass("row-selected");
			if ($(this).prop("checked")) $('.cbox-default').closest('tr').addClass("row-selected");
		});
		//********************************************//
		
		//*********** Activate/Deactivate script button from modal ***************//
		$(document).on( "click", "#btnActivate", function()
		{
			var active = $(this).data("active");
			var action = $(this).data("action");
			active = (active) ?	0 : 1;
			var id = $(this).data("id");
			var comments = $(".modal").find("#comments").val();
			var ticket = $(".modal").find("#txtJira").val();
			$(".modal").modal("hide");
			ajaxFormSubmit(action, "&id=" + id + "&n_activate=" + active + "&comments=" + comments + "&n_Jira=" + ticket, true);
			showWaitModal(500);
		});
		//********************************************//
		
		//*********** Activate/Deactivate script button from menu bar ***************//
		$(document).on( "click", "#mnuBtnEnable", function()
		{
			var modalDiv = $(this).data("target");
			if ($('.cbox-default:checked').length == 0) 
			{
				$("#errModal").modal("show");
				$(".modal").find(".modal-title").text("Informasjon");
				showInfoMessage("danger", "<b>Feil:</b> Ingen element er valgt", false, "#infoModal");
				return false;
			}
			
			$(modalDiv).modal("show");
		});
		//********************************************//
		
		//*********** Confirm copy button *************//
		$(document).on( "click", "#btnConfirmCopy", function()
		{
			var action = $(this).data("button-action");
			var param = "";
			var form_id = $(this).data("form");
			var serialize_form = $(form_id).serialize();
			var ids = [];
			
			$(".cbox-default:checked").each(function () 
			{
				ids.push($(this).val());
			});
			
			var counter = 0;
			for (var i in ids)
			{
				var id = ids[i];
				switch (action)
				{
					case "copy_testsuite":
						var include_tc = $("input#include_tc:checked").val();
						param = "&id=" + id + "&include-tc=" + include_tc;
						break;
					case "copy_testcase":
						param = serialize_form + "&id=" + id;
						break;
					case "copy_teststep":
					case "move_step":
						var ts_id = getUrlParam()["ts-id"];
						var tc_id = getUrlParam()["tc-id"];
						var copyOption = $("input[name='n_copy_step']:checked").val();
						var stepLocation = 1;
						if (copyOption == 2) stepLocation = $(this).data("last-step") + 1;
						else if (copyOption == 0) stepLocation = $("#txtStepNum").val();
						param = serialize_form + "&id=" + id + "&ts-id=" + ts_id + "&tc-id=" + tc_id + "&step-location=" + parseInt(parseInt(stepLocation) + counter);
						counter++;
						break;
				}
				ajaxFormSubmit(action, param, false);
			}
			
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** View testcases by testsuite ***************//
		$(document).on( "change", ".ts-list", function()
		{
			window.location.href = "?id=2&view-by=name&ts-id=" + $(this).val();
		});
		//********************************************//
		
		//*********** Move button from menu bar ***************//
		$(document).on( "click", "#mnuBtnMove", function()
		{
			var ts_id = getUrlParam()["ts-id"];
			var divModal = "#modalMove";
			$('.form-control option[value=' + ts_id + ']').remove();
			if ($('.cbox-default:checked').length == 0) 
			{
				$("#errModal").modal("show");
				$(".modal").find(".modal-title").text("Informasjon");
				showInfoMessage("danger", "<b>Feil:</b> Ingen element er valgt", false, "#infoModal");
				return false;
			}
			if (category == "teststeps")
			{
				divModal = "#modalStepCopyMove";
				var lblText1 = $("#lblCustom").text();
				var lblText2 = $("#lblTop").text();
				var lblText3 = $("#lblBottom").text();
				$(".modal").find(".modal-title").text("Flytte teststeg");
				$("#lblCustom").text(lblText1.replace("%label%", "Flytt"));
				$("#lblTop").text(lblText2.replace("%label%", "Flytt"));
				$("#lblBottom").text(lblText3.replace("%label%", "Flytt"));
				$(".modal").find("#btnConfirmCopy").attr("data-button-action", "move_step");
			}
			$(divModal).modal("show");
		});
		//********************************************//
		
		//*********** Confirm activate/deactivate button ***************//
		$(document).on( "click", "#btnConfirmActivate", function()
		{
			var form_id = $(this).data("form");
			var serialize_form = $(form_id).serialize();
			var action = $(this).data("button-action");
			$(".cbox-default:checked").each(function () 
			{
				ajaxFormSubmit(action, serialize_form + "&id=" + $(this).val(), false);
			});
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Confirm move button ***************//
		$(document).on( "click", "#btnConfirmMove", function()
		{
			var form_id = $(this).data("form");
			var serialize_form = $(form_id).serialize();
			var action = $(this).data("button-action");
			$(".cbox-default:checked").each(function () 
			{
				ajaxFormSubmit(action, serialize_form + "&id=" + $(this).val(), false);
			});
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Select category ***************//
		$(document).on( "change", "#cboCategory", function()
		{
			var category = $(this).val();
			ajaxFormSubmit("get_subcategory", "&category=" + category, false);
			$(".modal").find("#txtCommand").attr("placeholder", "");
		});
		//********************************************//
		
		//*********** Select category ***************//
		$(document).on( "change", "#cboSubCategory", function()
		{
			$("#txtCommand").val("");
			var cmdSubCategory = $("#cboSubCategory option:selected").text();
			$(".modal").find("#txtDescription").val("");
			if (cmdSubCategory == "Restart_Application" || cmdSubCategory == "Run_Windows_App")
			{
				if (cmdSubCategory == "Restart_Application") {$(".modal").find("#txtDescription").val("Start System X på nytt");}
				$(".modal").find("#txtCommand").val("[Eksternt Program][C:\\HMSCLIENT\\Systemx.exe]");
			}
			if (cmdSubCategory == "Application_Delay")
			{
				$(".modal").find("#txtCommand").val("[Delay-5]");
			}
			ajaxFormSubmit("get_test_command_info", "&command-id=" + $(this).val(), false);
		});
		//********************************************//
		
		//*********** Select window title ***************//
		$(document).on( "change", "#cboWindowTitle", function()
		{
			var winTitle = $(this).val();
			ajaxFormSubmit("get_objects", "&parent-id=" + winTitle, false);
		});
		//********************************************//
		
		//*********** Open Test Command Generator ***************//
		$(document).on( "click", "#divTCGenerator", function()
		{
			var cmdCategory = $("#cboCategory").val();
			var divIDContainer = "#divAppObject";
			var cmdSubCategory = $("#cboSubCategory option:selected").text();
			$(".div-gen").hide();
			if (cmdSubCategory == "" || cmdSubCategory == null)
			{
				showInfoMessage("danger", "Velg <b>Kategori</b> og <b>Sub-kategori</b> først");
				return false;
			}
			if (cmdCategory == "Application")
			{
				divIDContainer = "#divApplication";
			}
			
			if (cmdSubCategory == "Press_Keys_Exist" || cmdCategory == "Keyboard")
			{
				$("#divKeys").show();
			}
			else if (cmdSubCategory == "Object_Exist")
			{
				$("#divBoolean, #more-options-2").show();
			}
			else if (cmdSubCategory == "Object_Text")
			{
				$("#divObjText").show();
			}
			else if (cmdSubCategory == "Click_On_Object")
			{
				$("#chkAdvance").prop("checked", false);
				$("input[name='n_specific']").prop("checked",false);
				$(".object-text, .location, #more-options").hide();
				$("#txtLocation, #txtVisibleText").val("");
				$("#divObjClick").show();
			}
			else if (cmdSubCategory == "Window_Move")
			{
				$("#divLocation").show();
			}
			$(divIDContainer).show();
			$("#modalCommandGenerator").find('.modal-title').html("<span class='mdi mdi-codepen text-primary icon-md'></span> Test Command Generator");
			$("#modalCommandGenerator").modal("show");
		});
		//********************************************//
		
		//*********** Button ENTER: Test Command Generator ***************//
		$(document).on( "click", "#btnEnter", function()
		{
			var checkForm = validateGeneratorForm();
			var command = $("#txtCommand").val();
			if (!checkForm) return false;
			var cmdCategory = $("#cboCategory").val();
			var cmdSubCategory = $("#cboSubCategory option:selected").text();
			var enteredCommand = "";
			switch (cmdCategory)
			{
				case "Application":
					command = "";
					enteredCommand = "[Eksternt Program][" + $("#txtProgram").val() + "]";
					break;
				case "Checkpoint":
					enteredCommand = "[" + $("#cboWindowTitle option:selected").text() + "]" + "[" + $("#cboObjects option:selected").text() + "]";
					command = "";
					if (cmdSubCategory == "Object_Exist")
					{
						var boolExist = "[" + $("#modalCommandGenerator").find("input[name=n_object_exist]:checked").val() + "]";
						var filter = ($("#txtVisibleText2").val() != "") ? "[" + $("#txtVisibleText2").val() + "]" : "";
						enteredCommand = enteredCommand + filter + boolExist;
					}
					else if (cmdSubCategory == "Object_Text")
					{
						enteredCommand = enteredCommand + "[" + $("#txtObjText").val()  + "]";
					}
					else if (cmdSubCategory == "Press_Keys_Exist")
					{
						enteredCommand = enteredCommand + "[" + $("#txtKeys").val()  + "]";
					}
					break;
				case "Keyboard":
					enteredCommand = "[" + $("#cboWindowTitle option:selected").text() + "]" + "[" + $("#cboObjects option:selected").text() + "]";
					enteredCommand = enteredCommand + "[" + $("#txtKeys").val()  + "]";
					if (command != "") enteredCommand = ";" + enteredCommand;
					break;
				case "Mouse":
					enteredCommand = "[" + $("#cboWindowTitle option:selected").text() + "]" + "[" + $("#cboObjects option:selected").text() + "]";
					var objLocation = ($("#txtLocation").val() != "") ? "[" + $("#txtLocation").val()  + "]" : "";
					var objText = ($("#txtVisibleText").val() != "") ? "[" + $("#txtVisibleText").val()  + "]" : "";
					var objCoordinates = ($("#txtXY").val() != "") ? "[" + $("#txtXY").val()  + "]" : "";
					enteredCommand = enteredCommand + objLocation + objText + objCoordinates;
					if (command != "") enteredCommand = ";" + enteredCommand;
					break;
			}
			$("#txtCommand").val(command + enteredCommand);
			showInfoMessage("success", "Ferdig...", true, "#divGenerator");
		});
		//********************************************//
		
		//*********** Select condition ***************//
		$(document).on( "change", "#cboConditions", function()
		{
			var condition = ($(this).val() == "") ? $(this).val() : $(this).val() + "-";
			$("#txtObjText").val(condition);
		});
		//********************************************//
		
		//*********** Select condition ***************//
		$(document).on( "change", "#cboSpecialKeys", function()
		{
			var keys = $(this).val();
			var curr_keys = $("#txtKeys").val();
			$("#txtKeys").val(curr_keys + keys);
			$(this).prop('selectedIndex',0);
		});
		//********************************************//
		
		//*********** Select condition ***************//
		$(document).on( "change", "input[name='n_specific']", function()
		{
			var rb_selected = "." + $(this).val();
			$(".object-text, .location").hide();
			$(rb_selected).show();
		});
		//********************************************//
		
		//*********** Show more options ***************//
		$(document).on( "change", "#chkAdvance", function()
		{
			var selected = $(this).prop("checked");
			$("#more-options").hide();
			$("input[name='n_specific']").prop("checked",false);
			$(".object-text, .location").hide();
			if (selected) $("#more-options").show();
		});
		//********************************************//
		
		//*********** Select filter ***************//
		$(document).on( "change", "#cboFilter", function()
		{
			var filter = $(this).val() + "-";
			//var curr_filter = $("#txtVisibleText").val();
			$("#txtVisibleText").val(filter);
			$(this).prop('selectedIndex',0);
		});
		//********************************************//
		
		//*********** Show more options - Object_Exist ***************//
		$(document).on( "change", "#chkAdvance2", function()
		{
			var selected = $(this).prop("checked");
			$("#txtVisibleText2").val("");
			$("#divObjExistOptions").hide();
			if (selected) $("#divObjExistOptions").show();
		});
		//********************************************//
		
		//*********** Select filter - Object_Exist ***************//
		$(document).on( "change", "#cboFilter2", function()
		{
			var filter = $(this).val();
			$("#txtVisibleText2").val(filter);
			$(this).prop('selectedIndex',0);
		});
		//********************************************//
		
		//*********** Select destination on where the selected steps be copied to ***************//
		$(document).on( "change", "input[name='n_copy_step']", function()
		{
			var rbSelected = $(this).val();
			$("#txtStepNum").prop("disabled", true);
			$("#txtStepNum").prop("required", false);
			$("#txtStepNum").val("");
			if (rbSelected == 0)
			{
				$("#txtStepNum").prop("disabled", false);
				$("#txtStepNum").prop("required", true);
				$("#txtStepNum").focus();
			}
		});
		//********************************************//
		
		//*********** Inserting a new step ***************//
		$(document).on( "dblclick", ".test-step", function(e)
		{
			if (e.target.tagName == "I") return false;
			if (e.target.tagName != "LABEL" && e.target.tagName != "INPUT")
			{
				$(".modal").find("input:not([readonly])").val("");
				$(".modal").find("textarea").val("");
				$(".modal").find("select").prop('selectedIndex',0);
				var currentStep = $(this).data("step-number");
				$("#modalStepRegForm").modal("show");
				$("#stepNum").val(currentStep);
				$(".modal").find('#btnSave').attr("data-add-action", "insertNew");
				$(".modal").find('#btnSave').attr("data-action", "reg_teststeps");
				$(".modal").find("#modalTitle").html("<span class='mdi mdi-swap-vertical text-primary icon-md'></span> Sett inn nytt teststeg");
			}
		});
		//********************************************//
		
		//*********** View next or previous testcase ***************//
		$(document).on( "click", ".tc-navigator", function()
		{
			var direction = $(this).data("direction");
			var tcNum = $("#txtTcNum").val();
			if (stringContains($(this).attr("class"), "text-muted")) return false
			tcNum = (direction == "next") ? parseInt(tcNum) + 1 : parseInt(tcNum) - 1;
			var current_url = location.href;
			current_url = current_url.replace("tc-id=" + getUrlParam()["tc-id"], "tc-id=" + tcNum);
			window.location.href = current_url;
		});
		//********************************************//
		
		//*********** Click on testcase number textbox ***************//
		$(document).on( "click", "#txtTcNum, #txtTsNum", function()
		{
			$(this).select();
		});
		//********************************************//
		
		//*********** Enter testcase number to view ***************//
		$(document).on( "keypress", "#txtTcNum", function(e)
		{
			if (e.keyCode == 13)
			{
				var tcNum = $("#txtTcNum").val();
				var tsNum = $("#txtTsNum").val();
				var current_url = location.href;
				
				var lastTestcase = $("#txtTcNum").data("last-testcase");
				var lastTestsuite = $("#txtTsNum").data("last-testsuite");
				var error = 0;
				current_url = current_url.replace("tc-id=" + getUrlParam()["tc-id"], "tc-id=" + tcNum);
				current_url = current_url.replace("ts-id=" + getUrlParam()["ts-id"], "ts-id=" + tsNum);
				/*if (tcNum < 100 || tcNum > lastTestcase) error++;
				if (tsNum > lastTestsuite || tsNum < 1000) error++;
				if (error > 0)
				{
					$("#errModal").modal("show");
					showInfoMessage("danger", "<b>Feil:</b> Ugyldig testsuite eller testcase!", false, "#infoModal");
					return false;
				}*/
				window.location.href = current_url;
			}
		});
		//********************************************//
		
		//*********** Enter testcase number to view ***************//
		$(document).on( "keypress", "#txtTsNum", function(e)
		{
			if (e.keyCode == 13)
			{
				$("#txtTcNum").focus();
				$("#txtTcNum").select();
				var tsNum = $("#txtTsNum").val();
				ajaxFormSubmit("get_latest_testcase", "&ts-id=" + tsNum, false);
			}
		});
		//********************************************//
		
		//*********** Enter parent object name ***************//
		$(document).on( "focusout", "#txtWinTitle", function(e)
		{
			if ($(this).val() == "") return false;
			$(".modal").find("#btnSave").prop("disabled", false);
			ajaxFormSubmit("check_object_exist", "&name=" + $(this).val(), false);
		});
		//********************************************//
		
		//*********** View testcases by testsuite ***************//
		$(document).on( "change", ".object-list", function()
		{
			window.location.href = "?id=4&parent-id=" + $(this).val();
		});
		//********************************************//
		
		//*********** View testcases by testsuite ***************//
		$(document).on( "change", "#cboChildType", function()
		{
			$(".modal").find("#txtControlName").val($(this).val());
			$(".modal").find("#txtControlName").focus();
		});
		//********************************************//
		
		//*********** Enter child object name ***************//
		$(document).on( "focusout", "#txtControlName", function(e)
		{
			if ($(this).val() == "") return false;
			$(".modal").find("#btnSave").prop("disabled", false);
			var parentID = getUrlParam()["parent-id"];
			ajaxFormSubmit("check_child_object_exist", "&name=" + $(this).val() + "&parent-id=" + parentID, false);
		});
		//********************************************//
		
		//*********** Clean logs ***************//
		$(document).on( "click", "#btnCleanLogs", function()
		{
			ajaxFormSubmit("clean_logs", "", false);
			$(".modal").modal("hide");
			showWaitModal(1500);
		});
		//********************************************//
		
		//*********** Enable/Disable Threshold Limit ***************//
		$(document).on( "change", "input[name='nLimit']", function()
		{
			$("#divLimit").addClass("text-muted");
			$("#txtLimit").prop("disabled", true);
			if ($(this).val() == 1)
			{
				$("#divLimit").removeClass("text-muted");
				$("#txtLimit").prop("disabled", false);
				$("#txtLimit").focus();
			}
		});
		//********************************************//
		
		//*********** Confirm Threshold Limit ***************//
		$(document).on( "click", "#btnConfirmLimit", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			var serializeForm = $("#frmLogLimit").serialize();
			if ($("#txtLimit").prop("disabled") == false && $("#txtLimit").val() == 0)
			{
				showInfoMessage("danger", "<b>Error:</b> Must be greater than 0");
				return false;
			}
			$("#modalLogLimit").modal("hide");
			ajaxFormSubmit("activate_limit", serializeForm);
			showWaitModal(1500);
		});
		//********************************************//
		
		//*********** Select row ***************//
		$(document).on( "change", ".cbox-default", function()
		{
			$(this).closest('tr').removeClass("row-selected");
			if ($(this).prop("checked")) $(this).closest('tr').addClass("row-selected");
		});
		//********************************************//
		
		//*********** Confirm button -- Change database ***************//
		$(document).on( "click", "#btnChangeDB", function()
		{
			var serializeForm = $("#frmChangeDatabase").serialize();
			ajaxFormSubmit("change_database", serializeForm);
			$("#modalChangeDatabase").modal("hide");
			showWaitModal(1500);
		});
		//********************************************//
		
		//*********** View all tasks ***************//
		$(document).on( "click", "#spViewTask", function()
		{
			window.location.href = "?id=6";
		});
		//********************************************//
		
		//*********** View all tasks ***************//
		$(document).on( "click", "#spViewMessages", function()
		{
			window.location.href = "?id=17";
		});
		//********************************************//
		
		//*********** Button to send messages ***************//
		$(document).on( "click", "#btnSend", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			var serializeForm = $("#frmMessageForm").serialize();
			ajaxFormSubmit("send_message", serializeForm);
		});
		//********************************************//
		
		//*********** View selected message ***************//
		$(document).on( "click", ".row-message", function()
		{
			var msgId = $(this).data("message-id");
			window.location.href = "?id=17&message=" + msgId;
		});
		//********************************************//
		
		//*********** Activate/Deactivate user ***************//
		$(document).on( "click", "#mnuBtnActivate", function()
		{
			if ($('.cbox-default:checked').length == 0) 
			{
				$("#errModal").modal("show");
				$(".modal").find('#modalTitle').html("<span class='mdi mdi-alert text-warning icon-md'></span> Informasjon");
				showInfoMessage("danger", "<b>Feil:</b> Ingen elementer er valgt", false, "#infoModal");
				return false;
			}
			$("#modalActivateUser").modal("show");
		});
		//********************************************//
		
		//*********** Confirm activate/deactivate user***************//
		$(document).on( "click", "#btnConfirmActivateUser", function()
		{
			var serialize_form = $("#frmActivateToggleUser").serialize();
			$(".cbox-default:checked").each(function () 
			{
				ajaxFormSubmit("activate_user", serialize_form + "&username=" + $(this).val(), false);
			});
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Reset password ***************//
		$(document).on( "click", "#mnuBtnReset", function()
		{
			if ($('.cbox-default:checked').length == 0) 
			{
				$("#errModal").modal("show");
				$(".modal").find('#modalTitle').html("<span class='mdi mdi-alert-octagon text-danger icon-md'></span> <span class='text-danger'>Error</span> ");
				showInfoMessage("danger", "No user is selected!", false, "#infoModal");
				return false;
			}
			$("#modalResetPassword").modal("show");
			$(".modal").find('#modalTitle').html("<span class='mdi mdi-lock-reset text-warning icon-md'></span> <span class='text-danger'> Reset Password</span> ");
			var lenPassword = 10;
			var username = $(this).data("username");
			ajaxFormSubmit("create_password", "&length=" + lenPassword, false);
			$("#btnClose").text("No");
			$("#btnReset").show();
			$("#btnReset").attr("data-username", username);
		});
		//********************************************//
		
		//*********** Confirm Reset Password ***************//
		$(document).on( "click", "#btnReset", function()
		{
			var password = $(this).data("password").trim();
			$(".cbox-default:checked").each(function () 
			{
				ajaxFormSubmit("reset_password", "&username=" + $(this).val() + "&password=" + password, false);
			});
			
			$(".modal").find(".text-error").text("Password has been reset...");
			$(this).hide();
			$("#btnClose").text("Close");
		});
		//********************************************//
		
		//*********** Activate/Deactivate Testcase from Teststep Page ***************//
		$(document).on( "click", "#mnuBtnToggleActivate", function(e)
		{
			var tcTitle = $(".tc-title").text();			
			var tcComments = $(".tc-comments").text();
			tcComments = (tcComments == "Ingen kommentar") ? "" : tcComments;
			var tcActive = $(".tc-active").text();
			var btnText = "Aktiver";
			var btnClass = "primary";
			if (tcActive == "Ja")
			{
				btnText = "Deaktiver";
				btnClass = "danger";
			}
			$("#modalToggleTCActivate").modal("show");
			$(".modal").find('#modalTitle').html(tcTitle);
			$("#txtComments").val(tcComments);
			$(".modal").find('#btnToggleActivate').text(btnText);
			$(".modal").find('#btnToggleActivate').removeClass("btn-primary");
			$(".modal").find('#btnToggleActivate').removeClass("btn-danger");
			$(".modal").find('#btnToggleActivate').addClass("btn-" + btnClass);
		});
		//********************************************//
		
		$('#modalToggleTCActivate').on('shown.bs.modal', function ()
		{
			$(".modal").find("#txtComments").focus();
			$(".modal").find("#txtComments").select();
		});
		
		$(document).on( "click", ".modalClass", function()
		{
			var modalID = "#" + $(this).attr("id");
			var flag = false;
			var modalTitle = $(this).attr("title");
			var icon = $(this).data("icon");
			/*switch (modalID)
			{
				case "#modalUserProfile":
					modalTitle = "Min Profil";
					flag = true;
					break;
				case "#modalConfirmLogout":
					modalTitle = "Bekreft logg ut";
					flag = true;
					break;
				case "#modalChangeDatabase":
					modalTitle = "Bytt database";
					flag = true;
					break;	
			}
			if (flag)
			{
				$(".modal").find("#modalTitle").html(modalTitle);
			}*/
			$(".modal").find("#modalTitle").html("<span class='mdi mdi-" + icon + " text-primary icon-md'></span> " + modalTitle);
		});
		
		//*********** Confirm Activate/Deactivate Testcase ***************//
		$(document).on( "click", "#btnToggleActivate", function()
		{
			var tcComments = $("#txtComments").val();
			var tsID = getUrlParam()["ts-id"];
			var tcID = getUrlParam()["tc-id"];
			var tcActive = $(".modal").find('#btnToggleActivate').text();
			tcActive = (tcActive == "Aktiver") ? 1 : 0;
			ajaxFormSubmit("toggle_activate_testcase", "&ts-id=" + tsID + "&tc-id=" + tcID + "&comments=" + tcComments + "&activate=" + tcActive, false);
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Search Testcase ***************//
		$(document).on( "keyup", "#txtSearch", function()
		{
			var searchString = $(this).val();
			if (searchString.length == 0) 
			{
				$("#searchResult").removeClass("resultContainer"); 
				$("#searchResult").html("");
				return false;
			}
			
			$("#searchResult").addClass("resultContainer");
			//$("#searchResult").html(searchString);
			ajaxFormSubmit("search_testcase", "&keyword=" + searchString, false);
		});
		//********************************************//
		
		//*********** Search Testcase ***************//
		$(document).on( "keyup", "#txtSearchObject", function()
		{
			var searchString = $(this).val();
			if (searchString.length == 0) 
			{
				$("#searchObjResult").removeClass("resultContainer"); 
				$("#searchObjResult").html("");
				return false;
			}
			
			$("#searchObjResult").addClass("resultContainer");
			//$("#searchResult").html(searchString);
			ajaxFormSubmit("search_object", "&keyword=" + searchString, false);
		});
		//********************************************//
		
		//*********** Click on result ***************//
		$(document).on( "click", ".rowResult", function()
		{
			var tsID = $(this).data("ts-id");
			var tcID = $(this).data("tc-id");
			window.location.href = "?id=2&ts-id=" + tsID + "&tc-id=" + tcID;
		});
		//********************************************//
		
		//*********** Click on result ***************//
		$(document).on( "click", ".rowObjResult", function()
		{
			var parentID = $(this).data("parent-id");
			window.location.href = "?id=4&parent-id=" + parentID;
		});
		//********************************************//
		
		//*********** Change profile picture ***************//
		$(document).on( "click", "#imgAvatar", function()
		{
			$("#modalUserProfile").modal("hide");
		});
		//********************************************//
		
		//*********** Show activated/deactivated users ***************//
		$(document).on( "change", "input[name='rbOptions']", function()
		{
			window.location.href = "?id=8&show=" + $(this).val();
		});
		//********************************************//
		
		//*********** Show online/offline users ***************//
		$(document).on( "change", "input[name='rbShowOptions']", function()
		{
			window.location.href = "?id=7&show=" + $(this).val();
		});
		//********************************************//
		
		//*********** Show activated/deactivated testcases ***************//
		$(document).on( "click", ".mdi-crosshairs-gps, .mdi-crosshairs", function()
		{
			
			var active = ($(this).data("active") == "true") ? "false" : "true";
			window.location.href = "?id=1&show-active-only=" + $(this).data("active");
		});
		//********************************************//
		
		//*********** Click testsuite from teststeps page ***************//
		$(document).on( "click", "#pTestsuite", function()
		{
			var tcID = $(this).data("tc-id");
			var url = (window.location.href).replace("&tc-id=" + tcID, "");
			window.location.href = url;
		});
		//********************************************//
		
		//*********** Hover on menu item ***************//
		/*$(document).on( "mouseenter", ".menu-itemx", function()
		{
			var htmlContent = $(this).html();
			var pointer = "<i class=\"mdi mdi-hand-pointing-right\" style=\"position:fixed;margin-top:5px\"></i>";
			res = htmlContent.replace(pointer, "");
			$(this).html(pointer + res);
		});*/
		$(".menu-itemxxx").mouseenter(function(e)
		{
			var x = e.target.tagName;
			if (e.target.tagName == "I") return false;
			var htmlContent = $(this).html();
			var pointer = "<i class=\"mdi mdi-label\" style=\"position:fixed;margin-top:5px\"></i>";
			htmlContent = htmlContent.replace(pointer, "");
			$(this).prepend(pointer);
		});
		//********************************************//
		
		
		//*********** Mouseout on menu item ***************//
		$(".menu-itemxxx").mouseout(function()
		{
			var htmlContent = $(this).html();
			var pointer = "<i class=\"mdi mdi-label\" style=\"position:fixed;margin-top:5px\"></i>";
			htmlContent = htmlContent.replace(pointer, "");
			
			$(this).html(htmlContent);
		});
		//********************************************//
		
		//*********** Mouseout on menu item ***************//
		$(document).on( "contextmenu", "#aLogout", function()
		{
			$("#modalSessionTimeout2").modal("show");
			return false;
		});
		//********************************************//
		
		//*********** Change database ***************//
		$(document).on( "click", ".dbClass", function()
		{
			var database = $(this).data("dbname");
			ajaxFormSubmit("change_database", "&n_database=" + database);
			showWaitModal(1500);
		});
		//********************************************//
		
		//*********** Enclose selected text with b tag ***************//
		$(document).on( "keyup", "textarea, input", function(e)
		{
			var kc = e.which || e.keyCode;
			var textFormat = "";
			if (e.ctrlKey && String.fromCharCode(kc).toUpperCase() == "B") textFormat = "b";
			else if (e.ctrlKey && String.fromCharCode(kc).toUpperCase() == "I") textFormat = "i";
			else return false;
			var text = $(this);
			var currentText = text.val();
			var selectedText = text.val().substr(text[0].selectionStart, text[0].selectionEnd - text[0].selectionStart);
			var newText = currentText.replace(selectedText, "<" + textFormat + ">" + selectedText + "</" + textFormat + ">");
			if ((~selectedText.indexOf("<" + textFormat + ">")) || (~selectedText.indexOf("</" + textFormat + ">")))
			{
				var newSelectedText = selectedText.replace("<" + textFormat + ">", "");
				newSelectedText = newSelectedText.replace("</" + textFormat + ">", "");
				newText = currentText.replace(selectedText, newSelectedText);
			}
			
			if(selectedText != "") text.val(newText);
			else text.val(currentText + "<" + textFormat + "></" + textFormat + ">");
		});
		//********************************************//
		
		//*********** Change view type ***************//
		$(document).on( "change", "input[name='rbViewType']", function()
		{
			var viewType = $(this).val();
			$("#cboTestsuiteList").hide();
			$(".ts-input").hide();
			if (viewType == "name")
			{
				$("#cboTestsuiteList").show();
			}
			if (viewType == "id")
			{
				$(".ts-input").show();
				$(".ts-input").focus();
				$(".ts-input").select();
			}
		});
		//********************************************//
		
		//*********** Change view type ***************//
		$(document).on( "keyup", ".ts-input", function(e)
		{
			var tsID = $(this).val();
			if (e.which == 13)
			{
				tsID = tsID;
				if (tsID < 1000) return false;
			}
			else if (e.which == 38 || e.which == 39) tsID = parseInt(tsID) + 1;
			else if ((e.which == 37 || e.which == 40) && (tsID > 1000)) tsID = parseInt(tsID) - 1;
			else return false;
			window.location.href = "?id=2&view-by=id&ts-id=" + tsID;
		});
		//********************************************//
		
		//*********** Click event on Testsuite ID input ***************//
		$(document).on( "click", ".ts-input", function(e)
		{
			$(this).select();
		});
		//********************************************//
		
		//*********** Click event on Testsuite ID input ***************//
		$(document).on( "click", "#spnAlert", function(e)
		{
			$(".alert").fadeOut();
		});
		//********************************************//
		
		//*********** Action build ***************//
		$(document).on( "click", ".action-build", function()
		{
			var isLogin = $(this).data("is-login");
			var sxVersion = $(this).data("sxversion");
			var testSet = $(this).data("set");
			var title = $(this).attr("title");
			var buildID = $(this).data("build-id");
			$("#divAlert").fadeOut();
			if (isLogin)
			{
				if (title == "Kjøre denne jobben...") 
				{
					$(this).attr("src", "images/jenkins/ajax-loader.gif");
					ajaxFormSubmit("build_jenkins", "&sx-version=" + sxVersion + "&set=" + testSet);
				}
				else
				{
					if (confirm("Are you sure you want to abort System X " + sxVersion + " Ranorex Test " + testSet + " #" + buildID + "?"))
					{
						ajaxFormSubmit("stop_build_jenkins", "&sx-version=" + sxVersion + "&set=" + testSet + "&id=" + buildID);
						$(this).closest("tr").find("img").attr("src", "images/jenkins/aborted.png");
						$(this).attr("src", "images/jenkins/build.png");
						$(this).closest("tr").find("div").fadeOut();
						$(this).closest("tr").find(".blink").text("");
						$(this).attr("title", "Kjøre denne jobben...");
					}
				}
			}
			else
			{
				var buildAction = (title == "Kjøre denne jobben...") ? "run" : "stop";
				$("#modalLoginJenkins").modal("show");
				$(".modal").find('#modalTitle').text("Jenkins Login");
				$(".modal").find('#modalTitle').html("<img src='images/jenkins.png'> Koble til Jenkins");
				$(".modal").find('#btnConfirmJenkinsLogin').attr("data-sx-version", sxVersion);
				$(".modal").find('#btnConfirmJenkinsLogin').attr("data-set", testSet);
				$(".modal").find('#btnConfirmJenkinsLogin').attr("data-build-action", buildAction);
				$(".modal").find('#btnConfirmJenkinsLogin').attr("data-build-id", buildID);
			}
		});
		//********************************************//
		
		//*********** Stop build ***************//
		$(document).on( "click", ".stop-build", function()
		{
			var buildID = $(this).data("build-id");
			if (confirm("Er du sikker på at du vil avbryte System X " + sxVersion + " Ranorex Test " + testSet + " #" + buildID + "?"))
			{
				ajaxFormSubmit("stop_build_jenkins", "&sx-version=" + sxVersion + "&set=" + testSet + "&id=" + buildID);
				$(this).closest("tr").find("img").attr("src", "images/jenkins/aborted.png");
				$(this).attr("src", "images/jenkins/build.png");
				$(this).closest("tr").find("div").fadeOut();
				$(this).attr("title", "Kjøre denne jobben...");
			}
		});
		
		//*********** Click event on Testsuite ID input ***************//
		$(document).on( "click", ".progressbar-wrapper", function()
		{
			var divID = $(this).data("content-id");
			var sxVersion = $(this).data("version");
			var testSet = $(this).data("set");
			var buildID = $(this).data("build-id");
			ajaxFormSubmit("get_progress", "&sx-version=" + sxVersion + "&set=" + testSet + "&id=" + buildID + "&div-id=" + divID);
		});
		//********************************************//
		
		//*********** Show activated/deactivated testcases ***************//
		$(document).on( "click", "#mnuShowActiveOnly", function()
		{
			var tsID = getUrlParam()["ts-id"];
			var active = ($(this).data("active") == true) ? "false" : true;
			
			window.location.href = "?id=2&ts-id=" + tsID + "&show-active-only=" + active;
		});
		//********************************************//
		
		//*********** Activate/Deactivate Testsuite ***************//
		$(document).on( "click", "#btnDeactivateTS, #btnConfirmActivateTS", function()
		{
			var clickedBtn = $(this).attr("id");
			if (clickedBtn == "btnDeactivateTS")
			{
				var tsID = $(this).data("ts-id");
				var active = ($(this).text() == "Aktiver") ? 1 : 0;
				var strComments = $(".modal").find("#description").val();
				ajaxFormSubmit("activate_testsuite", "&ts-id=" + tsID + "&n_activate=" + active + "&comments=" + strComments);
			}
			else
			{
				var form_id = $(this).data("form");
				var serialize_form = $(form_id).serialize();
				var action = $(this).data("button-action");
				$(".cbox-default:checked").each(function () 
				{
					var tsID = $(this).data("ts-id");
					var strComments = (serialize_form == "n_activate=1") ? "": "This test is no longer in use";
					ajaxFormSubmit("activate_testsuite", serialize_form + "&ts-id=" + tsID + "&comments=" + strComments);
				});
			}
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Login to Jenkins ***************//
		$(document).on( "click", ".login-jenkins", function()
		{
			var sxVersion = $(this).data("sxversion");
			var testSet = $(this).data("set");
			var title = $(this).attr("title");
			var buildAction = $(this).data("build-action");
			var buildID = $(this).data("build-id");
			$("#modalLoginJenkins").modal("show");
			$(".modal").find('#modalTitle').text("Jenkins Login");
			$(".modal").find('#modalTitle').html("<img src='images/jenkins.png'> Koble til Jenkins");
			$(".modal").find('#btnConfirmJenkinsLogin').attr("data-sx-version", sxVersion);
			$(".modal").find('#btnConfirmJenkinsLogin').attr("data-set", testSet);
			$(".modal").find('#btnConfirmJenkinsLogin').attr("data-build-action", buildAction);
			$(".modal").find('#btnConfirmJenkinsLogin').attr("data-build-id", buildID);
		});
		//********************************************//
		
		//*********** Confirm Login to Jenkins ***************//
		$(document).on( "click", "#btnConfirmJenkinsLogin", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			
			var form_id = $(this).data("form");
			var serialize_form = $(form_id).serialize();
			var sxVersion = $(this).data("sx-version");
			var testSet = $(this).data("set");
			var buildAction = $(this).data("build-action");
			var buildID = $(this).data("build-id");
			//alert(serialize_form + "&sx-version=" + sxVersion + "&set=" + testSet + "&build-action=" + buildAction + "&build-id=" + buildID);
			ajaxFormSubmit("login_jenkins", serialize_form + "&sx-version=" + sxVersion + "&set=" + testSet + "&build-action=" + buildAction + "&build-id=" + buildID);
		});
		//********************************************//
		
		//*********** Toggle auto-refresh ***************//
		$(document).on( "click", ".mdi-refresh", function()
		{
			var currRefresh = $(this).data("auto-refresh");
			var autoRefresh = ($(this).data("auto-refresh")) ? "false" : "true";
			window.location.href = "?id=19&auto-refresh=" + autoRefresh;
		});
		//********************************************//
		
		//*********** Tab navigation ***************//
		$(document).on( "click", "a", function()
		{
			var toggle = $(this).data("toggle");
			if (toggle == "tab-menu")
			{
				var divID = $(this).attr("href");
				$(".tab-pane").hide();
				$(divID).fadeIn();
				$(this).closest("ul").find("li").removeClass("active");
				$(this).closest("li").addClass("active");
			}
		});
		//********************************************//
		
		//*********** Submit button: Change Order of Testcases ***************//
		$(document).on( "click", "#btnTab1", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			var serializeForm = $("#frmTab1").serialize();
			ajaxFormSubmit("change_tc_order", serializeForm);
		});
		//********************************************//
		
		//*********** Radio Button: Activate/Deactivate Testcases ***************//
		$(document).on( "change", "input[name='nOption']", function()
		{
			var rbText = $(this).data("action");
			var optionVal = $(this).val();
			$("label").removeClass("bold");
			$(this).closest(".formcontrol").find("label").addClass("bold");
			$("#btnTab2").text(rbText);
			$("#btnTab2").prop("disabled", false);
			$("#txtTCID").prop("disabled", true);
			$("#txtTCID").prop("required", false);
			$("#lblTC").text("Enter Testcase ID");
			if (optionVal >=3)
			{
				$("#txtTCID").prop("disabled", false);
				$("#txtTCID").prop("required", true);
				$("#lblTC").text("*Enter Testcase ID");
			}
			$("#txtTSID").focus();
		});
		//********************************************//
		
		//*********** Submit button: Activate/Deactivate Testcases ***************//
		$(document).on( "click", "#btnTab2", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			var serializeForm = $("#frmTab2").serialize();
			ajaxFormSubmit("activate_testcases", serializeForm);
		});
		//********************************************//
		
		//*********** Add Role Button (From Menu) ***************//
		$(document).on( "click", "#mnuBtnAddRole", function()
		{
			$("#btnDeleteRole").hide();
			$("#frmAddRoleForm").trigger("reset");
		});
		//********************************************//
		
		//*********** Save Role Button ***************//
		$(document).on( "click", "#btnSaveRole", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			var serializeForm = $("#frmAddRoleForm").serialize();
			var isVisible = $("#btnDeleteRole").is(':visible');
			if (isVisible)
			{
				ajaxFormSubmit("edit_role", serializeForm + "&id=" + $(this).data("record-id"));
			}
			else
			{
				ajaxFormSubmit("add_role", serializeForm);
			}
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Add Role Button (From Menu) ***************//
		$(document).on( "click", "#btnDeleteRole", function()
		{
			var id = $(this).data("record-id");
			var dlgConfirm = confirm("This will delete the entry permanently. Proceed?");
			$(".modal").modal("hide");
			if (dlgConfirm)
			{
				ajaxFormSubmit("delete_role", "&id=" + id);
				showWaitModal();
			}
		});
		//********************************************//
		
		//*********** Save User Button ***************//
		$(document).on( "click", "#btnSaveUser", function()
		{
			var check_form = validateForm();
			if (!check_form) return false;
			var serializeForm = $("#frmAddUserForm").serialize();
			var isVisible = $("#btnResetPassword").is(':visible');
			if (isVisible)
			{
				ajaxFormSubmit("edit_user", serializeForm + "&username=" + $(this).data("username"));
			}
			else
			{
				ajaxFormSubmit("add_user", serializeForm);
			}
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Activate/Deactivate a User ***************//
		$(document).on( "click", "#btnToggleActivateUser", function()
		{
			var username = $(this).data("username");
			var active = ($(this).text() == "Activate") ? 1 : 0;
			ajaxFormSubmit("activate_user", "&username=" + username + "&nToggleActivate=" + active);
			$(".modal").modal("hide");
			showWaitModal();
		});
		//********************************************//
		
		//*********** Reset Userpassword ***************//
		$(document).on( "click", "#btnResetPassword", function()
		{
			$(".user-password").show();
			ajaxFormSubmit("create_temp_password", "&length=10", false);
		});
		//********************************************//
		
		//*********** Select/Deselect checkboxes in Add Role form ***************//
		$(document).on( "change", "#chkSelectAll", function()
		{
			var check = $(this).prop("checked");
			$(".modal").find(":checkbox").prop("checked", check);
		});
		//********************************************//
		
		//*********** Enter data on Firstname field ***************//
		$(document).on( "keyup", "#txtFirstName", function()
		{
			$("#txtUserName").val($(this).val().toLowerCase());
		});
		//********************************************//
		
		//*********** Enter data on Lastname field ***************//
		$(document).on( "keyup", "#txtLastName", function()
		{
			$("#txtUserName").val($("#txtFirstName").val().toLowerCase() + "." + $(this).val().toLowerCase());
		});
		//********************************************//
	});
});

function validateForm(divId = "#info", showErrField=true)
{
	var error = false;
	var result = true;
	$('input,select,textarea').filter(':visible').each(function () 
	{
		$(this).css("border", "1px solid #CCC");
		$(this).css("box-shadow", "0 0 0px ");
	});
	
	$('input,select,textarea').filter('[required]:visible').each(function () 
	{
		
		$(this).css("border", "1px solid #CCC");
		$(this).css("background", "");
		if ($(this).val() == "" || $(this).val() == null)
		{
			if (showErrField)
			{
				$(this).css("border", "1px solid #FFCBA4");
				$(this).css("background", "#F2DEDE");
			}
			error = true;
		}
	});
	
	if (error)
	{
		showInfoMessage("danger", "<b>Feil:</b> Alle felter merket med * er påkrevd", true, divId);
		result =  false;
	}
	return result;
}

function validateGeneratorForm()
{
	var error = false;
	var result = true;
	$("#modalCommandGenerator").find('input,select').filter(':visible').each(function () 
	{
		$(this).css("border", "1px solid #CCC");
		$(this).css("box-shadow", "0 0 0px ");
	});
	
	$("#modalCommandGenerator").find('input,select').filter('[required]:visible').each(function () 
	{
		
		$(this).css("border", "1px solid #CCC");
		$(this).css("background", "");
		if ($(this).val() == "" || $(this).val() == null)
		{
			$(this).css("border", "1px solid #FFCBA4");
			$(this).css("background", "#F2DEDE");
			error = true;
		}
	});
	
	if (error)
	{
		showInfoMessage("danger", "<b>Feil:</b> Alle felt merket med * er påkrevd", true, "#divGenerator");
		result =  false;
	}
	return result;
}

function showWaitModal(timer=1500)
{
	$("#modalWait").modal("show");
	$(".modal-loading").delay(3000).modal("show");
	setTimeout(function () {document.location.reload();}, timer);
}

function getActiveModal()
{
	switch (category)
	{
		case "testsuite":
			modal_div = "#modalTsRegForm";
			break;
		case "testcase_all":
		case "testcase_single":
			modal_div = "#modalTcRegForm";
			break;
	}
	return modal_div;
}