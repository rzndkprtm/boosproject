<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Specification_Fabric_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Fabric Detail" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/specification/fabric">Fabric</a></li>
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
                                <a class="list-group-item list-group-item-action" id="listColour" data-bs-toggle="list" href="#list-colour" role="tab">Colour Data</a>
                            </div>
                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-general" role="tabpanel" aria-labelledby="listGeneral">
                                    <div class="border-bottom pb-3 mt-4 mb-4">
                                        <h2 class="fw-bold mb-1">
                                            <asp:Label runat="server" ID="lblName"></asp:Label>
                                        </h2>
                                        <div class="text-muted">Fabric Information</div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <h6 class="text-uppercase fw-bold text-primary mb-3">General Information</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Type</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblType"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Group</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblGroup"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">No Rail Road</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblNoRailRoad"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-4">
                                                <div class="col-4 text-muted">Status</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblStatus"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <h6 class="text-uppercase fw-bold text-success mb-3">Configuration</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Design Name</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblDesignName"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Tube Type</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblTubeType"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-4">
                                                <div class="col-4 text-muted">Company Detail</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblCompanyDetailName"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="mt-4" runat="server" id="divError">
                                        <div class="alert alert-danger mb-0">
                                            <span runat="server" id="msgError"></span>
                                        </div>
                                    </div>
                                    <div class="border-top pt-3 mt-4">
                                        <div class="d-flex flex-wrap gap-2">
                                            <asp:Button runat="server" ID="btnEditFabric" CssClass="btn btn-primary" Text="Edit Fabric" OnClick="btnEditFabric_Click" />
                                            <a href="#" runat="server" id="aChangeStatus" class="btn btn-outline-danger" data-bs-toggle="modal" data-bs-target="#modalChangeStatus">Change Status</a>
                                            <a href="javascript:void(0);" class="btn btn-outline-secondary" onclick="showLog('Fabrics', '<%= lblId.Text %>')">View Log</a>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane fade" id="list-colour" role="tabpanel" aria-labelledby="listColour">
                                    <div class="row mt-5" runat="server" id="divErrorColour">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorColour"></span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mt-4">
                                        <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                            <asp:Button runat="server" ID="btnAddColour" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddColour_Click" />
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                            <asp:Panel runat="server" DefaultButton="btnSearchColour" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Search : </span>
                                                    <asp:TextBox runat="server" ID="txtSearchColour" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearchColour" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchColour_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListColour" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" PageSize="100" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListColour_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="BoeId" HeaderText="BOE ID" />
                                                        <asp:BoundField DataField="Factory" HeaderText="Factory" />
                                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                                        <asp:BoundField DataField="Colour" HeaderText="Colour" />
                                                        <asp:BoundField DataField="Width" HeaderText="Width" />
                                                        <asp:BoundField DataField="Status" HeaderText="Status" />
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# LoginAccess("Detail Colour") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetailColour" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# LoginAccess("Change Status Colour") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalChangeStatusColour" onclick='<%# String.Format("return showChangeStatusColour(`{0}`, `{1}`, `{2}`);", Eval("Id").ToString(), Eval("Name").ToString(), Eval("Status").ToString()) %>'>Change Status</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('FabricColours', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                    <AlternatingRowStyle BackColor="White" />
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnChangeStatus" CssClass="btn btn-primary" Text="Submit" OnClick="btnChangeStatus_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalProcessColour" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 runat="server" class="modal-title" id="titleProcess"></h4>
                </div>
                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">BOE ID</label>
                            <asp:TextBox runat="server" ID="txtBoeId" CssClass="form-control" placeholder="BOE ID ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Factory</label>
                            <asp:DropDownList runat="server" ID="ddlFactoryColour" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Express" Text="Express"></asp:ListItem>
                                <asp:ListItem Value="Regular" Text="Regular"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Colour</label>
                            <asp:TextBox runat="server" ID="txtNameColour" CssClass="form-control" placeholder="Colour ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Width</label>
                            <asp:TextBox runat="server" ID="txtWidthColour" CssClass="form-control" placeholder="Width ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divStatusColour">
                        <div class="col-6 form-group">
                            <label class="form-label">Status</label>
                            <asp:DropDownList runat="server" ID="ddlStatusColour" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row mb-2" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcess"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessColour" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcessColour_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal fade text-left" id="modalChangeStatusColour" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Change Status</h4>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdStatusColour" style="display:none;"></asp:TextBox>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtChangeName" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Old Status</label>
                            <asp:DropDownList runat="server" ID="ddlOldStatusColour" CssClass="form-select" Enabled="false">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="In Stock" Text="In Stock"></asp:ListItem>
                                <asp:ListItem Value="Limited Stock" Text="Limited Stock"></asp:ListItem>
                                <asp:ListItem Value="Out of Stock" Text="Out of Stock"></asp:ListItem>
                                <asp:ListItem Value="Discontinued" Text="Discontinued"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">New Status</label>
                            <asp:DropDownList runat="server" ID="ddlNewStatusColour" CssClass="form-select">
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnChangeStatusColour" CssClass="btn btn-primary" Text="Submit" OnClick="btnChangeStatusColour_Click" />
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
        <asp:Label runat="server" ID="lblIdColour"></asp:Label>
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
            $("#listColour").on("click", function () {
                updateSessionValue("list-colour");
            });
        });

        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvListColour.ClientID %>');
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

                    const btn = this.querySelector("a[id*='linkDetailColour']");
                    if (btn) btn.click();
                });
            }
        });

        function showProcessColour() {
            $("#modalProcessColour").modal("show");
        }

        function showChangeStatusColour(id, name, status) {
            document.getElementById("<%=txtIdStatusColour.ClientID %>").value = id;
            document.getElementById("<%=txtChangeName.ClientID %>").value = name;
            document.getElementById("<%=ddlOldStatusColour.ClientID %>").value = status;
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

        ["modalChangeStatus", "modalProcessColour", "modalChangeStatusColour", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>