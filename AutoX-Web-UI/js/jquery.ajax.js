function ajaxFormSubmit(action, serialize_form, show_info=true)
{
	var msg_type = "success";
	$.post( 
		  "ajax-data-processor.php",
		  serialize_form + "&action=" + action,
		  function(data) {
				if (data.indexOf("Feil:") >= 0) 
				{
					msg_type = "danger";
				}
				else if (data.indexOf("ERROR:") >= 0) 
				{
					msg_type = "danger";
				}
				
				if (show_info) 
				{
					showInfoMessage(msg_type, data, true);
					if (msg_type == "Feil")
					{
						return false;
					}
				}
				if (action == "get_subcategory")
				{
					$("#divSubCategory").html(data);
				}
				else if (action == "get_objects")
				{
					$("#divObjects").html(data);
				}
				else if (action == "get_latest_testcase")
				{
					data = data.trim();
					$("#txtTcNum").attr("data-last-testcase", data);
				}
				else if (action == "check_object_exist" && msg_type == "danger")
				{
					showInfoMessage(msg_type, data, true);
					$("#txtWinTitle").select();
					$("#txtWinTitle").focus();
					$(".modal").find("#btnSave").prop("disabled", true);
				}
				else if (action == "check_child_object_exist" && msg_type == "danger")
				{
					showInfoMessage(msg_type, data, true);
					$(".modal").find("#txtControlName").select();
					$(".modal").find("#txtControlName").focus();
					$(".modal").find("#btnSave").prop("disabled", true);
				}
				else if (action == "create_password")
				{
					$(".modal").find(".text-error").html("The new password is <i class='bold'>" + data + "</i>. Confirm?");
					$(".modal").find("#btnReset").attr("data-password", data);
				}
				else if (action == "get_test_command_info")
				{
					$(".modal").find("#txtCommand").attr("placeholder", data);
				}
				else if (action == "search_testcase")
				{
					$("#searchResult").html(data);
				}
				else if (action == "search_object")
				{
					$("#searchObjResult").html(data);
				}
				else if (action == "build_jenkins")
				{
					if (msg_type == "danger")
					{
						$("#divAlert").fadeIn();
						$("#divAlert").html(data);
						$(".run-build").attr("src", "images/jenkins/build.png");
					}
					else { showWaitModal(1500); }
				}
				else if (action == "get_progress")
				{
					var result = data.split(":");
					$(result[1]).css("width", result[0] + "%");
					$(result[1]).html(result[0] + "%");
					$(result[1]).closest("td").find("div").attr("title", result[0] + "% fullført...");
					if (parseInt(result[0]) >= 100) 
					{ 
						showWaitModal(1500); 
					}
				}
				else if (action == "login_jenkins")
				{
					if (data.indexOf("SUCCESS") >= 0)
					{
						$(".modal").modal("hide");
						showWaitModal(1500);
					}
					else
					{
						$(".modal").find("#loginError").fadeIn();
						$(".modal").find("#loginError").html(data);
					}
				}
				else if (action == "create_temp_password")
				{
					$("#txtPassword").val(data);
				}
		  }
	   );
}

function ajaxSearchApp(application)
{
	$.post( 
		  "ajax-search-engine.php",
		  "&application=" + application,
		  function(data) {
				
				$("#i_prog_list").html(data);
				$("#i_prog_list").hide();
				$("#i_prog_list").fadeToggle();
		  }
	   );
}

function ajaxFormUsers(action, serialize_form)
{
	var msg_type = "success";
	$.post( 
		  "ajax-user-processor.php",
		  serialize_form + "&action=" + action,
		  function(data) {
				var show_info = true;
				
				if (data.indexOf("Feil:") >= 0) 
				{
					msg_type = "danger";
				}
				
				if ((msg_type == "success") && (action == "login"))
				{
					if (localStorage.getItem('name') == null) setTimeout(function () {window.location.href = "/autox-web-ui";}, 2000);
					else setTimeout(function () {window.location.href = localStorage.getItem('name');}, 2000);
				}
				if ((msg_type == "success") && (action == "create_user"))
				{
					setTimeout(function () {window.location.href = "login.php";}, 2000);
				}
				
				if ((msg_type == "Feil") && (action == "login"))
				{
					showInfoMessage2("danger", "<b>Feil:</b> Ugyldig brukernavn eller passord");
				}
				if ((msg_type == "success") && (action == "update_profile"))
				{
					if (data.indexOf("Vellykket") >= 0) showWaitModal(500);
				}
				if (show_info) 
				{
					showInfoMessage2(msg_type, data);
					if (msg_type == "Feil")
					{
						return false;
					}
				}
				
		  }
	   );
}

function ajax_manager(serialize_form)
{
	var nClass = "";
	$.post( 
		  "ajax-tc-manager.php",
		  serialize_form,
		  function(data) {
				if (data.indexOf("ERROR:") >= 0) 
				{
					nClass = "error";
				}
				
					$("#infoMsg").hide().addClass(nClass).html(data).fadeToggle();
				
		  }
	   );
}

