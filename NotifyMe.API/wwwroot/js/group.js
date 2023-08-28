function editing(entityId) {
    const url = '/Groups/Edit';
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
        success: function(data){
            $('#placeholder-modal-edit').html(data);
            $('#modal-edit').modal('show');
            console.log(data);
        },
        error: function(){
            console.log("Something went wrong with editing");
        }
    });
}

function creating() {
    const url = '/Groups/Create';
    $.ajax({
        url: url,
        type: 'GET',
        headers: {
            "Content-Type": "application/json",
            "Cache-Control": "no-cache, no-store",
            "Pragma": "no-cache",
            "X-Frame-Options": "SAMEORIGIN",
        },
        success: function(data){
            $('#placeholder-modal-create').html(data);
            $('#modal-create').modal('show');
            console.log(data);
        },
        error: function(){
            console.log("Something went wrong with creating");
        }
    });
}

function previewing(entityId) {
    const url = '/Groups/Details';
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
        success: function(data){
            $('#placeholder-modal-details').html(data);
            $('#modal-details').modal('show');
            console.log(data);
        },
        error: function(){
            console.log("Something went wrong with previewing");
        }
    });
}

function deleting(entityId) {
    const url = '/Groups/Delete';
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
            console.log("Something went wrong with deleting");
        }
    });
}