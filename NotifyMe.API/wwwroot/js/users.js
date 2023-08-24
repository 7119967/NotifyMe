function deleting(entityId) {
    $.ajax({
        url: '@Url.Action("Delete", "Users")',
        type: 'DELETE',
        data: { 'entityId': entityId },
        headers: {
            "Content-Type": "application/json",
        },
        success: function (data) {
            console.log(data);
        },
        error: function (error) {
            console.log(error);
            alert(error.status + '\n' + error.message);
        }
    });
}