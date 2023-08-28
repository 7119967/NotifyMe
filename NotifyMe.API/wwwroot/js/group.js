function editing(entityId) {
    $.ajax({
        url: '@Url.Action("Edit", "Groups")',
        type: 'GET',
        headers: {
            "Content-Type": "application/json",
        },
        data: { 'entityId': entityId },
        success: function(data){
            $('#placeholder-modal-edit').html(data);
            // $('#placeholder-modal-edit > .modal-edit', data).modal('show');
            $('#modal-edit').modal('show');
            console.log(data);
            // $('#modal-edit').html(output);
            // $('#modal-edit').modal('show');
            // $('#modal-edit').show();
            // $("#modal-edit").css("overflow", "auto");
            // $("body").css("overflow", "hidden");
        },
        error: function(){
            console.log("Something went wrong");
        }
    });
}

function creating() {
    $.ajax({
        url: '@Url.Action("Create", "Groups")',
        type: 'POST',
        data:  $("#form-GroupCreate").serialize(),
    });
}

function previewing(entityId) {
    $.ajax({
        url: '@Url.Action("Details", "Groups")',
        type: 'GET',
        data: { 'entityId': entityId },
        success: function(data){
            $('#placeholder-modal-details').html(data);
            // $('#placeholder-modal-edit > .modal-edit', data).modal('show');
            $('#modal-details').modal('show');
            console.log(data);
            // $('#modal-edit').html(output);
            // $('#modal-edit').modal('show');
            // $('#modal-edit').show();
            // $("#modal-edit").css("overflow", "auto");
            // $("body").css("overflow", "hidden");
        },
        error: function(){
            console.log("Something went wrong");
        }
    });
}

function deleting(entityId) {
    const url = '/Groups/Delete';
    $.ajax({
        url: url,
        type: 'GET',
        data: { 'entityId': entityId },
        success: function(data){
            $('#placeholder-modal-delete').html(data);
            // $('#placeholder-modal-edit > .modal-edit', data).modal('show');
            $('#modal-delete').modal('show');
            console.log(data);
            // $('#modal-edit').html(output);
            // $('#modal-edit').modal('show');
            // $('#modal-edit').show();
            // $("#modal-edit").css("overflow", "auto");
            // $("body").css("overflow", "hidden");
        },
        error: function(){
            console.log("Something went wrong");
        }
    });
}