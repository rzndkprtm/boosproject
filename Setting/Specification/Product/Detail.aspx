<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Specification_Product_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Product Detail" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification">Specification</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/product">Product</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>
    <div class="page-content">
        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                <a class="list-group-item list-group-item-action active" id="listGeneral" data-bs-toggle="list" href="#list-general" role="tab">General Data</a>
                                <a class="list-group-item list-group-item-action" id="listKit" data-bs-toggle="list" href="#list-kit" role="tab">Kit Data</a>
                            </div>
                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-general" role="tabpanel" aria-labelledby="listGeneral">
                                    <div class="border-bottom pb-3 mt-4 mb-4">
                                        <h2 class="fw-bold mb-1">
                                            <asp:Label runat="server" ID="lblName"></asp:Label>
                                        </h2>
                                        <div class="text-muted">
                                            <asp:Label runat="server" ID="lblStatus"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <h6 class="text-uppercase fw-bold text-primary mb-3">Product Information</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Design Name</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblDesignName"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Blind Name</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblBlindName"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Invoice Name</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblInvoiceName"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-4">
                                                <div class="col-4 text-muted">Company Detail</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblCompanyName"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <h6 class="text-uppercase fw-bold text-success mb-3">Configuration</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Tube Type</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblTubeType"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Control Type</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblControlType"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Colour Type</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblColourType"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mt-4">
                                        <h6 class="text-uppercase fw-bold text-secondary mb-3">Description</h6>
                                        <div class="border rounded p-3 bg-light">
                                            <asp:Label runat="server" ID="lblDescription"></asp:Label>
                                        </div>
                                    </div>
                                    <div class="mt-4" runat="server" id="divError">
                                        <div class="alert alert-danger mb-0">
                                            <span runat="server" id="msgError"></span>
                                        </div>
                                    </div>
                                    <div class="border-top pt-3 mt-4">
                                        <div class="d-flex flex-wrap gap-2">
                                            <asp:Button runat="server" ID="btnEditProduct" CssClass="btn btn-primary" Text="Edit Product" OnClick="btnEditProduct_Click" />
                                            <a href="javascript:void(0);" runat="server" id="aChangeStatus" class="btn btn-outline-danger" data-bs-toggle="modal" data-bs-target="#modalChangeStatus">Change Status</a>
                                            <a href="javascript:void(0);" class="btn btn-outline-secondary" onclick="showLog('Products', '<%= lblId.Text %>')">View Log</a>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane fade" id="list-kit" role="tabpanel" aria-labelledby="listKit">
                                    <div class="row mt-5" runat="server" id="divErrorKit">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorKit"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-4">
                                        <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                            <asp:Button runat="server" ID="btnAddKit" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddKit_Click" />
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvList_RowCommand">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="KitId" HeaderText="KIT ID" />
                                                        <asp:BoundField DataField="VenId" HeaderText="VEN ID" />
                                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                                        <asp:BoundField DataField="BlindStatus" HeaderText="Blind Status" />
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# LoginAccess("Detail Kit") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# LoginAccess("Delete Kit") %>'>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteKit" onclick='<%# String.Format("return showDeleteKit(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('ProductKits', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalChangeStatus" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Change Status</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">New Status</label>
                            <asp:DropDownList runat="server" ID="ddlNewStatus" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnChangeStatus" CssClass="btn btn-primary" Text="Submit" OnClick="btnChangeStatus_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalProcessKit" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 runat="server" class="modal-title" id="titleProcess"></h4>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-4 form-group">
                            <label class="form-label">HK ID</label>
                            <asp:TextBox runat="server" ID="txtKitId" CssClass="form-control" placeholder="KIT ID ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-4 form-group">
                            <label class="form-label">Venetian ID</label>
                            <asp:TextBox runat="server" ID="txtVenId" CssClass="form-control" placeholder="VEN ID ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-4 form-group">
                            <label class="form-label">Blind Status</label>
                            <asp:DropDownList runat="server" ID="ddlBlindStatus" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Control" Text="Control"></asp:ListItem>
                                <asp:ListItem Value="Middle" Text="Middle"></asp:ListItem>
                                <asp:ListItem Value="End" Text="End"></asp:ListItem>
                                <asp:ListItem Value="Metal" Text="Metal"></asp:ListItem>
                                <asp:ListItem Value="Semi Metal" Text="Semi Metal"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtNameKit" CssClass="form-control" placeholder="Name ..." autocomplete="off" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Custom Name</label>
                            <asp:TextBox runat="server" ID="txtCustomName" CssClass="form-control" placeholder="Custom Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorProcessKit">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessKit"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessKit" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcessKit_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-center" id="modalDeleteKit" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Product Kit</h5>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdDeleteKit" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteKit" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteKit_Click" />
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

    <asp:HiddenField ID="selected_tab" runat="server" />

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblIdKit"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            var selectedTab = $("#<%=selected_tab.ClientID%>");
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "list-general";
            $('#dvTab a[href="#' + tabId + '"]').tab('show');
            $("#dvTab a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });
            $("#listGeneral").on("click", function () {
                updateSessionValue("list-general");
            });
            $("#listKit").on("click", function () {
                updateSessionValue("list-kit");
            });
        });

        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) return;

            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';

                row.addEventListener('click', function (e) {
                    if (
                        e.target.closest("a") ||
                        e.target.closest("button") ||
                        e.target.closest("[data-bs-toggle]")
                    ) {
                        return;
                    }

                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                });
            }
        });

        function showProcessKit() {
            $("#modalProcessKit").modal("show");
        }

        function showDeleteKit(id) {
            document.getElementById("<%=txtIdDeleteKit.ClientID %>").value = id;
        }

        function updateSessionValue(session) {
            $.ajax({
                type: "POST",
                url: "Detail.aspx/UpdateSession",
                data: JSON.stringify({ value: session }),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
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

        ["modalChangeStatus", "modalProcessKit", "modalDeleteKit", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
