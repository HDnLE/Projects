jQuery(function($)
{
	$(document).ready(function() 
	{
		///////// Login Button /////////
		$("#btn_login").click(function(event)
		{
			validateUserLogin();
		});
		
		///////// Keyboard Login /////////
		$(document).on( "keydown", "#key_login", function(event) 
		{
			var key = event.which;
			if (key == 13)
			{
				validateUserLogin();
			}
		});
		
		///////// Sign-up Button /////////
		$(document).on( "click", "#btn_create_user", function() 
		{
			var result;
			var serialize_form = $("#frm_create_user").serialize();
			alert(serialize_form);
			if ($("input[name='n_password']").val() != $("input[name='n_confirm_password']").val())
			{
				showInfoMessage("danger", "<b>Feil:</b> Passordene er ikke like", true);
				return false;
			}
			if (validateSignupForm())
			{
				ajaxFormUsers("create_user", serialize_form);
			}
		});
		
		function validateSignupForm()
		{
			var error = false;
			$('input,select').filter(':visible').each(function () 
			{
				$(this).css("border", "1px solid #CCC");
				$(this).css("box-shadow", "0 0 0px ");
			});
			$('input,select').filter('[required]:visible').each(function () 
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
				showInfoMessage("danger", "<b>Feil:</b> Alle felt må fylles ut", true);
				return false;
			}
			if ($("input[name='n_password']").val() != $("input[name='n_confirm_password']").val())
			{
				showInfoMessage("danger", "<b>Feil:</b> Passordene er ikke like", true);
				return false;
			}
			return true;
		}
		
		///////// Update Profile Button /////////
		$(document).on( "click", "#update", function()
		{
			var serialize_form = $("#frm_update_profile").serialize();
			$("#info").fadeOut();
			$("#info").html("");
			
			$("input:password").prop("required", false);
			var toggle_change = $("input:checkbox").prop("checked");
			if (toggle_change)
			{
				$("input:password").prop("required", true);
			}
			if (validateSignupForm())
			{
				ajaxFormUsers("update_profile", serialize_form);
			}
		});
		
		///////// Change Password Checkbox /////////
		$(document).on( "change", "input:checkbox:not('#enter'):not('#escape')", function()
		{
			var toggle_change = $(this).prop("checked");
			$("input:password").prop("disabled", true);
			$("label").slice(-2).css("color", "#AAA");
			$("input:password").val("");
			$("input:password").css("background", "");
			$("input:password").css("border", "1px solid #CCC");
			if (toggle_change)
			{
				$("input:password").prop("disabled", false);
				$("label").slice(-2).css("color", "#000");
				$("input:password").eq(0).focus();
			}
		});	
		
		//*********** Update Profile ***************//
		$(document).on( "click", "#btnUpdateProfile", function()
		{
			var serializeForm = $("#frmUserProfile").serialize();
			if (validateSignupForm())
			{
				ajaxFormUsers("update_profile", serializeForm);
				$(".modal").modal("hide");
				showWaitModal(1500);
			}
			
			
		});
		//********************************************//
		
		//*********** Select Pre-defined avatar ***************//
		$(document).on( "click", ".avatar", function()
		{
			$("#imgAvatar").attr("src", ($(this).attr("src")));
			$("#modalProfilePhoto, #modalUserProfile").modal("hide");
			//$("input[name='nAvatar']").val($(this).attr("src"));
			ajaxFormSubmit("update_profile_photo", "&photo=" + $(this).attr("src"), false);
			showWaitModal(1500);
		});
		//********************************************//
		
		//*********** Select Pre-defined avatar ***************//
		$(document).on( "click", "#btnUpload", function()
		{
			$.ajax({
			url: "ajax_php_file.php", // Url to which the request is send
			type: "POST",             // Type of request to be send, called as method
			data: new FormData($('#frmProfilePhoto')[0]), // Data sent to server, a set of key/value pairs (i.e. form fields and values)
			contentType: false,       // The content type used when sending data to the server.
			cache: false,             // To unable request pages to be cached
			processData:false,        // To send DOMDocument or non processed data file it is set to false
			success: function(data)   // A function to be called if request succeeds
			{
				var msgType = "success";
				if (data.indexOf("ERROR:") >= 0) 
				{
					msgType = "danger";
				}
				showInfoMessage(msgType, data, true, "#infoUpload");
				if (msgType == "success")
				{
					$("#modalProfilePhoto, #modalUserProfile").modal("hide");
					showWaitModal(1500);
				}
			}
			});
		});
		//********************************************//
	});
});

//Check the login form for empty fields
function validateUserLogin()
{
	var result;
	var serialize_form = $("#frm_login").serialize();
	var database = $( "select option:selected" ).text();
	
	$('input').each(function() 
	{
		
		if(!$(this).val()){
			result = true;
		}
		
	});
	
	
	if (result)
	{
		showInfoMessage2("danger", "<b>Feil:</b> Ingen brukernavn eller passord er gitt!");
		return false;
	}
	if (database == "")
	{
		showInfoMessage2("danger", "<b>Feil:</b> Ingen database valgt!");
		return false;
	}
	ajaxFormUsers("login", serialize_form);
}