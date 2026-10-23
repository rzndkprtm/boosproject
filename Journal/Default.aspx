<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Journal_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Journal" %>

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
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row mb-2" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>
        <section class="row mb-3">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-secondary" Text="Add New" OnClick="btnAdd_Click" />
            </div>
        </section>
        <section class="row">
            <div class="col-12">
                <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="card">
                            <div class="card-header">
                                <div class="row align-items-center">
                                    <div class="col-12 col-sm-12 col-lg-8 mb-2 mb-lg-0">
                                        <h5 class="card-title">List Journal</h5>
                                    </div>
                                    <div class="col-12 col-sm-12 col-lg-4">
                                        <div class="row g-2">
                                            <div class="col-12 col-md-7">
                                                <asp:DropDownList runat="server" ID="ddlSales" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlSales_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div class="col-12 col-md-5">
                                                <asp:TextBox runat="server" ID="txtDate" CssClass="form-control" TextMode="Date" AutoPostBack="true" OnTextChanged="txtDate_TextChanged" onkeydown="return false;" onpaste="return false;" ondrop="return false;"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="card-body">
                                <div class="table-responsive">
                                    <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Visible="false" OnPageIndexChanging="gvList_PageIndexChanging" OnDataBound="gvList_DataBound">
                                        <Columns>
                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <%# Container.DataItemIndex + 1 %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                            <asp:BoundField DataField="CreatedByFull" HeaderText="Created By" />
                                            <asp:BoundField DataField="Activity" HeaderText="Activity" />
                                            <asp:BoundField DataField="CustomerName" HeaderText="Customer Name" />
                                            
                                            <asp:TemplateField HeaderText="Description" ItemStyle-Width="550px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblDescription" runat="server" Text='<%# If(String.IsNullOrEmpty(If(Eval("Description"), "").ToString()), "-", If(If(Eval("Description"), "").ToString().Length > 100, Server.HtmlEncode(If(Eval("Description"), "").ToString().Substring(0, 100)) & " ", Server.HtmlEncode(If(Eval("Description"), "").ToString()))) %>'></asp:Label>
                                                    <asp:LinkButton ID="btnMoreDescription" runat="server" Text="Full Description" CssClass="text-primary text-decoration-none" CausesValidation="false" Visible='<%# If(Eval("Description"), "").ToString().Length > 100 %>' OnClientClick='<%# "showDescription(" & System.Web.HttpUtility.JavaScriptStringEncode(If(Eval("Description"), "").ToString(), True) & "); return false;" %>'></asp:LinkButton>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="110px">
                                                <ItemTemplate>
                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                    <ul class="dropdown-menu">
                                                        <li runat="server" visible='<%# (If(Eval("Status"), "").ToString() = "Open") AndAlso LoginAccess("Edit") %>'>
                                                            <a class="dropdown-item" id="aEdit" href='<%# Page.ResolveUrl("~/journal/edit?journalid=" & Eval("Id")) %>'>Edit</a>
                                                        </li>
                                                        <li runat="server" visible='<%# (If(Eval("Status"), "").ToString() = "Open") AndAlso LoginAccess("Delete") %>'>
                                                            <a href="javascript:void(0);" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return dataDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                        </li>
                                                        <li>
                                                            <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('Journals', '<%# Eval("Id") %>')">Log</a>
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
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </section>
    </div>

    <div class="modal fade text-center" id="modalWaiting" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-body text-center py-4">
                    <div class="spinner-border text-primary" role="status">
                        <span class="visually-hidden">Loading...</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade" id="modalDescription" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalDescriptionLabel">Full Description</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div id="fullDescription" style="white-space: pre-wrap; overflow-wrap: anywhere;"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Journal</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtDeleteId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDelete_Click" />
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
                bindDatePicker();
            });
        }
        function showDescription(description) {
            document.getElementById("fullDescription").textContent = description || "-";
            $("#modalDescription").modal("show");
        }
        function bindDatePicker() {
            const dateInput = document.getElementById('<%= txtDate.ClientID %>');
            if (!dateInput || dateInput.dataset.bound === "true") return;

            dateInput.dataset.bound = "true";

            dateInput.addEventListener("keydown", function (e) {
                e.preventDefault();
            });
            dateInput.addEventListener("paste", function (e) {
                e.preventDefault();
            });
            dateInput.addEventListener("drop", function (e) {
                e.preventDefault();
            });
        }
        document.addEventListener("DOMContentLoaded", function () {
            initUpdatePanelLoading();
            bindDatePicker();
        });
        function dataDelete(id) {
            document.getElementById("<%=txtDeleteId.ClientID %>").value = id;
        }
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
        ["modalDescription", "modalDelete", "modalLog"].forEach(function (id) {
            const modal = document.getElementById(id);

            if (modal) {
                modal.addEventListener("hide.bs.modal", function () {
                    document.activeElement.blur();
                    document.body.focus();
                });
            }
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>