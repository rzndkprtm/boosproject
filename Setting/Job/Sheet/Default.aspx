<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Setting_Job_Sheet_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Job Sheet" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="page-heading">
        <div class="page-title">
            <div class="row">
                <div class="col-12 col-md-6 order-md-1 order-last">
                    <h3><%: Page.Title %></h3>
                    <p class="text-subtitle text-muted"></p>
                </div>
                <div class="col-12 col-md-6 order-md-2 order-first">
                    <nav aria-label="breadcrumb" class="breadcrumb-header float-start float-lg-end">
                        <ol class="breadcrumb">
                            <li class="breadcrumb-item"><a runat="server" href="~/">Home</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting">Setting</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/job">Job</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row mb-3" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-12 d-flex flex-wrap justify-content-end gap-1">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-content">
                                <div class="card-header">
                                    <div class="row">
                                        <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                            <h4 class="card-title">List Job Sheet</h4>
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                            <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Search : </span>
                                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </div>
                                <div class="card-body">
                                    <div class="row mb-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Visible="false" OnPageIndexChanging="gvList_PageIndexChanging" OnDataBound="gvList_DataBound">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                                        <asp:BoundField DataField="Alias" HeaderText="Alias" />
                                                        <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <a class="dropdown-item" id="aEdit" href='<%# Page.ResolveUrl("~/setting/job/sheet/detail/?sheetid=" & Eval("Id")) %>'>Detail</a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# LoginAccess("Edit") %>'>
                                                                        <a class="dropdown-item" id="aDetail" href='<%# Page.ResolveUrl("~/setting/job/sheet/edit?sheetid=" & Eval("Id")) %>'>Edit</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('JobSheets', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                            <div class="d-flex justify-content-end mt-3">
                                                <nav id="navPager" runat="server" visible="false">
                                                    <ul class="pagination pagination mb-0">
                                                        <asp:Repeater ID="rptPager" runat="server" OnItemCommand="rptPager_ItemCommand">
                                                            <ItemTemplate>
                                                                <li class='page-item <%# Eval("CssClass") %>'>
                                                                    <asp:LinkButton runat="server" ID="lnkPage" CssClass="page-link" Text='<%# Eval("Text") %>' CommandName="Page" CommandArgument='<%# Eval("PageIndex") %>' />
                                                                </li>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </ul>
                                                </nav>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Product</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" data-bs-backdrop="static" data-bs-keyboard="false" role="dialog" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Changelog</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger d-none" id="logError"></div>
                    <div class="table-responsive">
                        <table class="table table-vcenter card-table" id="tblLogs">
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
    </div>

    <script type="text/javascript">
        window.addEventListener("pageshow", function () {
            var loading = document.getElementById("loadingOverlay");
            if (loading) loading.style.display = "none";
        });
        function initUpdatePanelLoading() {
            if (typeof Sys === "undefined") return;
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "block";
            });
            prm.add_endRequest(function () {
                var loading = document.getElementById("loadingOverlay");
                if (loading) loading.style.display = "none";
                bindGridRowClick();
            });
        }

        function bindGridRowClick() {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) return;
            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = "pointer";
                row.onclick = function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                        return;
                    }
                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                };
            }
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            bindGridRowClick();
        });
        function showLog(type, dataId) {
            $("#logError").addClass("d-none").html("");
            $("#tblLogs tbody").html("");
            $("#modalLog").modal("show");

            $.ajax({
                type: "POST",
                url: "/Setting/Method.aspx/GetLogs",
                data: JSON.stringify({ type: type, dataId: dataId }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    const logs = res.d;

                    if (!logs || logs.length === 0) {
                        $("#tblLogs tbody").html(
                            `<tr><td class="text-center">DATA LOG NOT FOUND</td></tr>`
                        );
                        return;
                    }

                    let html = "";
                    logs.forEach(r => {
                        html += `<tr><td>${r.TextLog}</td></tr>`;
                    });

                    $("#tblLogs tbody").html(html);
                },
                error: function (err) {
                    $("#logError").removeClass("d-none").html("FAILED TO LOAD LOG DATA");
                }
            });
        }
        ["modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>