$(function ()
{
    $('#ManagePageElementPermissions').on('shown.bs.modal', managePageElementPermissionsShown);
});

function savePageElement(buttonJObject)
{
    $('button').attr('disabled', '');

    var form = buttonJObject.closest('form');
    var countryID = form.find("[name='CountryID']").val();
    var pageElementID = form.find("[name='PageElementID']").val();

    var data = {};
    $(form).find('[name]').each(function (index, element)
    {
        element = $(element);

        if (element.attr('type') == 'checkbox')
        {
            data[element.attr('name')] = element[0].checked;
        }
        else
        {
            data[element.attr('name')] = element.val();
        }
    });

    cydon.ajax.sendData('/PageEditor/PageElementSave/' + countryID + '/' + pageElementID, data, function (data)
    {
        if (!data.success)
        {
            for (var key in data.errors)
            {
                var control = form.find("[data-validate-message-for='" + key + "']");
                if (!control.length)
                {
                    control = form.find('.genericError');
                }

                control.text(data.errors[key]);
            }
        }
    }, null, function ()
    {
        $('button').removeAttr('disabled');
    });
}

function deletePageElement(buttonJObject)
{
    $('button').attr('disabled', '');

    var form = buttonJObject.closest('div').find('form');
    var countryID = form.find("[name='CountryID']").val();
    var pageElementID = form.find("[name='PageElementID']").val();

    var deleteModal = $('#DeletePageElementModal');
    deleteModal.find('button').removeAttr('disabled');
    deleteModal.find('#DeletePageElementButton').attr('onclick', "performDeletePageElement(" + countryID + ", " + pageElementID + ");");
    deleteModal.modal('show');
}

function performDeletePageElement(countryID, pageElementID)
{
    var form = $('PageElement-' + pageElementID);
    cydon.ajax.sendData('/PageEditor/PageElementDelete/' + countryID + '/' + pageElementID, null, function (data)
    {
        if (!data.success)
        {
            for (var key in data.errors)
            {
                var control = form.find("[data-validate-message-for='" + key + "']");
                if (!control.length)
                {
                    control = form.find('.genericError');
                }

                control.text(data.errors[key]);
            }
        }
        else
        {
            var div = $('#PageElement-' + pageElementID).parent('div');
            div.fadeOut(300, function ()
            {
                div.remove();
            });
        }
    }, null, function ()
    {
        $('button').removeAttr('disabled');
        $('#DeletePageElementModal').modal('hide');
    });
}

function movePageElementUp(buttonJObject)
{
    $('button').attr('disabled', '');

    var form = buttonJObject.closest('div').find('form');
    var countryID = form.find("[name='CountryID']").val();
    var pageElementID = form.find("[name='PageElementID']").val();

    cydon.ajax.sendData('/PageEditor/PageElementMoveUp/' + countryID + '/' + pageElementID, null, function (data)
    {
        if (!data.success)
        {
            for (var key in data.errors)
            {
                var control = form.find("[data-validate-message-for='" + key + "']");
                if (!control.length)
                {
                    control = form.find('.genericError');
                }

                control.text(data.errors[key]);
            }
        }
        else
        {
            var div = buttonJObject.closest('div');
            div.prev().before(div);

            // Side logic
            //var currentDiv = buttonJObject.closest('div');
            //var aboveDiv = currentDiv.prev();

            //var currentDivHeight = currentDiv.height();
            //var aboveDivHeight = aboveDiv.height();

            //var currentDivPosition = currentDiv.position();
            //var aboveDivPosition = aboveDiv.position();

            //currentDiv.addClass('position-absolute');
            //aboveDiv.addClass('position-absolute');

            //currentDiv.animate({
            //    "top": currentDiv.position().top - currentDivHeight
            //});
            //aboveDiv.animate({
            //    "top": aboveDiv.position().top + aboveDivHeight
            //}, null, null, function ()
            //{
            //    currentDiv.removeClass('position-absolute');
            //    aboveDiv.removeClass('position-absolute');

            //    aboveDiv.before(currentDiv);
            //});
        }
    }, null, function ()
    {
        $('button').removeAttr('disabled');
    });
}

function movePageElementDown(buttonJObject)
{
    $('button').attr('disabled', '');

    var form = buttonJObject.closest('div').find('form');
    var countryID = form.find("[name='CountryID']").val();
    var pageElementID = form.find("[name='PageElementID']").val();

    cydon.ajax.sendData('/PageEditor/PageElementMoveDown/' + countryID + '/' + pageElementID, null, function (data)
    {
        if (!data.success)
        {
            for (var key in data.errors)
            {
                var control = form.find("[data-validate-message-for='" + key + "']");
                if (!control.length)
                {
                    control = form.find('.genericError');
                }

                control.text(data.errors[key]);
            }
        }
        else
        {
            var div = buttonJObject.closest('div');
            div.next().after(div);
        }
    }, null, function ()
    {
        $('button').removeAttr('disabled');
    });
}

function managePageElementPermissionsShown(e)
{
    var modal = $(e.target);
    var countryID = $('#CountryID').val();
    var pageElementID = modal.find('input[name="PageElementID"]').val();

    var loading = modal.find('.loading');
    var failed = modal.find('.failed');
    var permissionsContent = modal.find('.permissionsContent');
    var controlButtons = modal.find('.modal-footer').find('button');

    loading.addClass('d-flex');
    loading.removeClass('d-none');
    failed.addClass('d-none');
    permissionsContent.addClass('d-none');
    controlButtons.attr('disabled', '');

    cydon.ajax.sendData('/PageEditor/PageElementLoadPerms/' + countryID + '/' + pageElementID, { PageElementID: pageElementID }, function (data)
    {
        loading.addClass('d-none');
        loading.removeClass('d-flex');
        if (!data.success)
        {
            failed.removeClass('d-none');
            return;
        }

        permissionsContent.removeClass('d-none');
        permissionsContent.text('');
        permissionsContent.append(data.content);
        controlButtons.removeAttr('disabled');
    });
}

function managePageElementPermissionsAddCountryRole(buttonJObject)
{
    var table = buttonJObject.prevAll('table');
    var tbody = table.find('tbody');
    var rowCount = tbody.find('tr').length - 1;
    var templateRow = table.find('.templateRow');
    var html = $(templateRow[0].outerHTML);
    html.removeClass('d-none');
    html = html[0].outerHTML;
    html = html.replace(/template-perm/g, "new-perm-" + rowCount);
    html = $(html);
    html.removeClass('templateRow');
    html.addClass('newRow');
    tbody.append(html);
}

function managePageElementPermissionsRemoveCountryRole(buttonJObject)
{
    var row = buttonJObject.parents('tr');

    if (row.hasClass('newRow'))
    {
        row.remove();
        return;
    }

    row.addClass('d-none');
    row[0].outerHTML = row[0].outerHTML.replace(/perm-/g, 'removed-perm-');
}

//var formeditor =
//{
//    radiobutton:
//    {
//        addQuestion: function (buttonJObject)
//        {
//            var newHtml = buttonJObject.parent().find('.answer-template')[0].outerHTML;
//            var newTextBox = $(newHtml).removeClass('answer-template').removeClass('d-none').insertBefore(buttonJObject).find('[id$="answer-0"]');
//            var parts = newTextBox.attr('id').split('-');
//            var number = buttonJObject.parentsUntil('.radio-group-answers').find('.form-row').length - 1; // Minus 1 to account for template row
//            parts[parts.length - 1] = number;

//            var newId = '';
//            for (var i = 0; i < parts.length; i++)
//            {
//                if (i != 0)
//                {
//                    newId += '-';
//                }

//                newId += parts[i];
//            }

//            newTextBox.attr('id', newId).attr('name', newId);
//        },
//        removeQuestion: function (buttonJObject)
//        {
//            var parentRow = buttonJObject.parents('.form-row');
//            parentRow.animate({
//                opacity: 0
//            }, 500, null, function ()
//            {
//                parentRow.addClass('d-none');
//                var childTextBox = parentRow.find('input[type="text"]');
//                childTextBox.attr('id', childTextBox.attr('id') + '-removed');
//                childTextBox.attr('name', childTextBox.attr('name') + '-removed');
//            });
//        }
//    },
//    deleteFormElement: function (formElementID, buttonJObject)
//    {
//        $('button').attr('disabled', '');

//        var countryID = $('#CountryID').val();

//        cydon.ajax.sendData('/PageEditor/FormElementDelete/' + countryID);
//    }
//};