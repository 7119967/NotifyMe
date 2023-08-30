﻿function editing(entityId) {
    const url = '/Users/Edit';
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
            // $('#modal-edit').html(output);
            // $('#modal-edit').modal('show');
            // $('#modal-edit').show();
            // $("#modal-edit").css("overflow", "auto");
            // $("body").css("overflow", "hidden");
            console.log(data);
        },
        error: function(){
            console.log("Something went wrong with editing");
        }
    });
}

function creating() {
    const url = '/Users/Create';
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
    const url = '/Users/Details';
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
    const url = '/Users/Delete';
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
            $('#modal-delete').modal('show');
            console.log(data);
        },
        error: function(){
            console.log("Something went wrong with deleting");
        }
    });
}