jQuery(function($)
{
	$(document).ready(function()
	{
		$(document).on( "click", ".close", function()
		{
			$("#info").fadeOut();
			$("#info").html("");
		});
		
		$(document).on( "click", "input:text", function()
		{
			if ($(this).prop("readonly"))
			{
				$(this).blur();
				return false;
			}
			if ($(this).attr("id") == "page") {return false;}
			$("input:text").css("box-shadow", "0 0 0px");
			$(this).css("box-shadow", "0 0 5px Gold");
		});
		
		$(document).on( "click", "#logout", function()
		{
			var username = $("#username").text().split("(")[1].split(")")[0];
			ajax_form_submit("logout", "&username=" + username);
		});
		
		$(document).on( "mouseover", "[data-type='tooltip']", function()
		{
			
			if ($(this).data("tooltip") != undefined) 
			{
				var leftMargin = "0px";
				var tooltip = $(this).data("tooltip") + "...";
				var width = "auto";
				if ($(this).position().left >= 1400)
				{
					leftMargin = "-150px";
					width = "170px";
				}
				var item = $("<label id='tooltip' class='tooltip_' style='width:" + width + ";margin-left:" + leftMargin + "'></label>").hide().fadeIn(500);
				
				// style='width:" + width + ";position:absolute;margin-top:-25px;margin-left:" + leftMargin + ";background:#000;padding:4px;border-radius:3px;border:1px solid #DDD;box-shadow: 3px 3px 3px #DDD;font-weight:normal;font-family:Arial;font-size:12px'
				$(this).append(item);
				$("#tooltip").html(tooltip);
				
			}
		});
		$(document).on( "mouseout", "[data-type='tooltip']", function()
		{
			//$("#tooltip").html("&nbsp;");
			$("#tooltip").remove();
		});
	});
	
	
	
	$(document).on( "keypress", "#i_step_num, #page, #pageNav", function(event)
	{
		var allowed_keys = event.which == 8 || event.which == 0;
		
		if (!allowed_keys)
		{
			if (event.which < 48 || event.which > 57) {event.preventDefault();}
		}
	});
	
	$(document).on( "keypress", "input[name='n_ts_title'], input[name='n_tc_title']", function(event)
	{
		var keys = event.which;
		var invalid_keys = (keys >= 33 && keys <= 43) || keys == 61 || keys == 47 || keys == 63 || (keys >= 91 && keys <= 95) || (keys >= 123 && keys <= 126);
		
		if (invalid_keys)
		{
			$("#info").html("Tittelen bør ikke inneholde noen spesialtegn");
			$("#info").removeClass("success").addClass("error");
			$("#info").fadeIn();
			$("#info").delay(2000).fadeOut();
			event.preventDefault();
		}
	});
});
function showInfoMessage(message_type, message, auto_close=true, target_display="#info")
{	
	jQuery(function($)
	{
		$(".modal").find(target_display).hide();
		$(".modal").find(target_display).html("");
		$(".modal, .tabs").find(target_display).fadeIn();
		$(".modal, .tabs").find(target_display).html("<div class='text-13 alert text-" + message_type + "'>" + message + "</div>");
		if (auto_close)
		{$(".modal, .tabs").find(target_display).delay(2000).fadeOut();}
	});
}

function showInfoMessage2(msg_type, message)
{
	$("#info").hide();
	$("#info").html("");
	$("#info").fadeIn();
	$("#info").html("<div style='position:fixed;width:100%' class='text-13 alert alert-" + msg_type + "'>" + message + "</div>");
	//$("#info").delay(2000).fadeOut();
}

function extractTestData(data)
{
	var matches = [];

	var pattern = /\[(.*?)\]/g;
	var match;
	while ((match = pattern.exec(data)) != null)
	{
	  matches.push(match[1]);
	}

	return matches;
}

function stringContains(strText, strSearch)
{
	var result = false;
	if (strText.indexOf(strSearch) >= 0)
	{
		result = true;
	}
	return result;
}

String.prototype.encloseBracket = function()
{
	if (this != "")
	{
		return this.replace(this, "[" + this + "]");
	}
	return "";
};

function getUrlParam()
{
	/*var arr = document.URL.match(/id=([0-9]+)/);
	if (arr[1] == null) arr[1] = -1; 
	return arr[1];*/
    var vars = {};
    var parts = window.location.href.replace(/[?&]+([^=&]+)=([^&]*)/gi,    
    function(m,key,value) {
      vars[key] = value;
    });
    return vars;
}