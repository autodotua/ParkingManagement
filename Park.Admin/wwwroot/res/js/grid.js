//使用 otherActionButtonActions(cnode, rowData)函数来指定表格中的链接按钮的事件
//多选删除按钮需要指定click:onDeleteSelectedClick监听器
//需要添加    <input type="hidden" id="urls" data-postback="@Url.Handler("DoPostBack")" />

function onDeleteSelectedClick() {
    var grid1 = F.ui.Grid1;

    if (!grid1.hasSelection()) {
        F.alert('请至少选择一项！');
        return;
    }
    url = $("#urls").data("postback");
    console.log(url)
    var selectedRows = grid1.getSelectedRows();
    F.confirm({
        message: '你确定要删除选中的&nbsp;<b>' + selectedRows.length + '</b>&nbsp;行数据吗？',
        target: '_top',
        ok: function () {
            // 触发后台事件
            F.doPostBack(url, 'Panel1', {
                actionType: 'delete',
                deletedRowIDs: selectedRows
            });
        }
    });
}

function renderButtons(btns) {
    let content = "";
    for (let btn of btns) {
        content = content + '<a class="action-btn ' + btn.class + '" href="javascript:;">' + btn.text + '</a>';
    }
    return content;
}



F.ready(function () {

    var grid1 = F.ui.Grid1;
    grid1.el.on('click', 'a.action-btn', function (event) {
        var cnode = $(this);
        var rowData = grid1.getRowData(cnode.closest('.f-grid-row'));

        // 是否禁用
        if (cnode.hasClass('f-state-disabled')) {
            return;
        }
        url = $("#urls").data("postback");
        if (cnode.hasClass('delete')) {
            F.confirm({
                message: '确定删除此记录？',
                target: '_top',
                ok: function () {
                    // 触发后台事件
                    F.doPostBack(url, 'Panel1', {
                        actionType: 'delete',
                        deletedRowIDs: [rowData.id]
                    });
                }
            });
        }
        if (otherActionButtonActions) {//其它按钮
            otherActionButtonActions(cnode, rowData);
        }
    });
       
});