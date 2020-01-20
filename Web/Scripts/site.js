var virtualPath = $('#virtualpath').val();
var cydon = new Object();
cydon.timeout = new Object();
cydon.timeout.idleMinutes = 0;

cydon.ajax =
{
    sendData: function (url, data, success, error, complete)
    {
        cydon.timeout.idleMinutes = 0;

        $.ajax({
            url: virtualPath + url,
            method: 'POST',
            data: data,
            success: success,
            error: error,
            complete: complete
        });
    }
};

$("[type='submit']").click(disableSubmitButtons);

function disableSubmitButtons(event)
{
    var submits = $("[type='submit']");
    submits.addClass('disabled');

    submits.each(function (index, submit)
    {
        submit.setAttribute('disabled', null);
    });

    if (event.target)
    {
        var button = $(event.target);
        if (button.parents('form'))
        {
            button.parents('form').submit();
        }
    }
}

$(function ()
{
    cydon.timeout.interval = setInterval(function ()
    {
        cydon.timeout.idleMinutes++;
        if (cydon.timeout.idleMinutes >= 25)
        {
            var remainingMinutes = 30 - cydon.timeout.idleMinutes;

            if (remainingMinutes < 0)
            {
                remainingMinutes = 0;
            }

            if (remainingMinutes == 0)
            {
                location.href = virtualPath + '/CySys/SignOut';
            }

            $('#inactiveMinutes').text(cydon.timeout.idleMinutes);
            $('#inactiveRemainingMinutes').text(remainingMinutes);

            $('#timeoutDialog').modal('show');
        }
    }, 60000);
});

$('#refreshSession').click(function (index, button)
{
    $('#refreshSession').attr('disabled');
    $('#closeSession').attr('disabled');
    cydon.ajax.sendData('/CySys/RefreshSession', null, function (data)
    {
        if (data.success)
        {
            $('#timeoutDialog').modal('hide');

            cydon.timeout.idleMinutes = 0;
        }
        else
        {
            location.href = virtualPath + '/CySys/SignOut';
        }

        $('#refreshSession').removeAttr('disabled');
        $('#closeSession').removeAttr('disabled');
    }, function (jqXHR, textStatus, errorThrown)
    {
        alert('An internal server error occurred:\n' + jqXHR + '\n' + textStatus + '\n' + errorThrown);
    });
});

$('#closeSession').click(function ()
{
    $('#refreshSession').attr('disabled');
    $('#closeSession').attr('disabled');
    location.href = virtualPath + '/CySys/SignOut';
});

$('#deleteButton').click(function ()
{
    $('#deleteButton').addClass('disabled');
    $('#cancelDelete').attr('disabled', '');
});

function confirmDelete(deleteRoute, objectNameOverride)
{
    $('#confirmDelete #deleteButton').attr('href', virtualPath + '/' + deleteRoute);

    if (objectNameOverride)
    {
        $('#confirmDelete #objectName').text(objectNameOverride);
    }

    $('#confirmDelete').modal('show');
}

function modalSave(saveRoute, caller, customSuccessCallback)
{
    var buttons = caller.closest('.modal-footer').find('button');
    $(buttons).attr('disabled', '');

    var inputs = caller.closest('.modal').find('[name]');

    var data = {};
    inputs.each(function (index, input)
    {
        input = $(input);
        var value = input.val();

        if (input.attr('type') === 'checkbox')
        {
            value = input[0].checked;
        }
        data[input.attr('name')] = value;
    });

    cydon.ajax.sendData(saveRoute, data, function (data)
    {
        if (data.success)
        {
            if (customSuccessCallback)
            {
                customSuccessCallback();
            }
            else
            {
                window.location.reload();
            }
        }

        if (data.errors)
        {
            for (var key in data.errors)
            {
                $("[data-validate-message-for='" + key + "']").text(data.errors[key]);
            }
        }
    }, null, function ()
    {
        var buttons = caller.closest('.modal-footer').find('button');
        $(buttons).removeAttr('disabled');
    });
}