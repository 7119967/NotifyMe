function editing(entityId) {
    const url = '/Configurations/Edit';
    $.ajax({
        url: url,
        type: 'GET',
        headers: {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache, no-store",
            "Pragma": "no-cache",
            "X-Frame-Options": "SAMEORIGIN",
        },
        data: { 'entityId': entityId },
        success: function (result) {
            $('#placeholder-modal-edit').html(result);
            $('#modal-edit').modal('show');
            console.log(result);
        },
        error: function () {
            console.log("Something went wrong with editing");
        }
    });
}

function creating() {
    const url = '/Configurations/Create';
    $.ajax({
        url: url,
        type: 'GET',
        headers: {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache, no-store",
            "Pragma": "no-cache",
            "X-Frame-Options": "SAMEORIGIN",
        },
        success: function (result) {
            $('#placeholder-modal-create').html(result);
            $('#modal-create').modal('show');
            console.log(result);
        },
        error: function () {
            console.log("Something went wrong with creating");
        }
    });
}

function previewing(entityId) {
    const url = '/Configurations/Details';
    $.ajax({
        url: url,
        type: 'GET',
        headers: {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache, no-store",
            "Pragma": "no-cache",
            "X-Frame-Options": "SAMEORIGIN",
        },
        data: { 'entityId': entityId },
        success: function (result) {
            $('#placeholder-modal-details').html(result);
            $('#modal-details').modal('show');
            console.log(result);
        },
        error: function () {
            console.log("Something went wrong with previewing");
        }
    });
}

function deleting(entityId) {
    const url = '/Configurations/Delete';
    $.ajax({
        url: url,
        type: 'GET',
        headers: {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache, no-store",
            "Pragma": "no-cache",
            "X-Frame-Options": "SAMEORIGIN",
        },
        data: { 'entityId': entityId },
        success: function (result) {
            $('#placeholder-modal-delete').html(result);
            $('#modal-delete').modal('show');
            console.log(result);
        },
        error: function () {
            console.log("Something went wrong with deleting");
        }
    });
}