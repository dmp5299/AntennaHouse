function loadGrid(id, title, stage) {
    $('#jqGrid').jqGrid('GridUnload');
    $("#jqGrid").jqGrid({
        url: "/Home/GetComments",
        datatype: 'json',
        postData: { id1: id },
        mtype: 'POST',
        colNames: ['CommentId', 'BookId', 'Item No.', 'Page', 'Comment', 'V/I', 'Class. Code'],
        colModel: [
            { key: true, hidden: true, name: 'CommentId', index: 'CommentId', editable: true },
            { key: true, hidden: true, name: 'BookId', index: 'BookId', editable: true },
            { key: false, name: 'ItemNumber', index: 'ItemNumber', editable: true, width: 25 },
            { key: false, name: 'Page', index: 'Page', editable: true, width: 25 },
            { key: false, name: 'Comment1', index: 'Comment1', editable: true, width: 150 },
            { key: false, name: 'ValidStatus', index: 'ValidStatus', editable: true, edittype: 'select', editoptions: { value: { '1': 'V', '2': 'I', '3': 'OOS' } }, width: 25 },
            { key: false, name: 'ClassCode', index: 'ClassCode', editable: true, width: 25 }],
        pager: '#jqControls',
        rowNum: 10,
        rowList: [10, 20, 30, 40, 50],
        height: '100%',
        width: '50%',
        viewrecords: true,
        caption: title + ' ' + stage,
        emptyrecords: 'No comments for this book yet',
        jsonReader: {
            root: "rows",
            page: "page",
            total: "total",
            records: "records",
            repeatitems: false,
            Id: "0"
        },
        autowidth: true,
        multiselect: false
    }).navGrid('#jqControls', {
        add: true,
        edit: true,
        del: true,
        search: true,
        viewrecords: true
    },
    {
        url: '/Home/AddComment',
        closeOnEscape: true,
        closeOnExit: true,
        recreateForm: true,
        afterComplete: function () {
            alert("row added!");
        }
    }
    );
}