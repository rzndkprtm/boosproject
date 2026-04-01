<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Alias.aspx.vb" Inherits="Setting_Specification_Fabric_Alias" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Fabric Alias" %>

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
                                <a class="list-group-item list-group-item-action active" id="listFabric" data-bs-toggle="list" href="#list-fabric" role="tab">Fabric Type</a>
                                <a class="list-group-item list-group-item-action" id="listFabricColour" data-bs-toggle="list" href="#list-fabriccolour" role="tab">Fabric Colour</a>
                            </div>

                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-fabric" role="tabpanel" aria-labelledby="listFabric">
                                    <div class="row mt-5" runat="server" id="divError">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgError"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12 col-sm-12 col-lg-6">
                                            <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
                                        </div>
                                        <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                            <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Search : </span>
                                                    <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="grid-container table-responsive">
                                                <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvList_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="FirstName" HeaderText="Fabric Name" />
                                                        <asp:BoundField DataField="SecondName" HeaderText="Fabric Name" />
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Delete") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('FabricAlias', '<%# Eval("Id") %>')">Log</a>
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

                                <div class="tab-pane fade" id="list-fabriccolour" role="tabpanel" aria-labelledby="listFabricColour">
                                    <div class="row mt-5" runat="server" id="divErrorColour">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorColour"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12 col-sm-12 col-lg-6">
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
                                            <div class="grid-container table-responsive">
                                                <asp:GridView runat="server" ID="gvListColour" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListColour_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="FirstName" HeaderText="Fabric Name" />
                                                        <asp:BoundField DataField="SecondName" HeaderText="Fabric Name" />
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Delete") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteColour" onclick='<%# String.Format("return showDeleteColour(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('FabricColourAlias', '<%# Eval("Id") %>')">Log</a>
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

    <div class="modal fade text-left" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 runat="server" class="modal-title" id="titleProcess"></h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-5">
                        <div class="col-6 form-group">
                            <label class="form-label">First ID</label>
                            <asp:DropDownList runat="server" ID="ddlFirstId" CssClass="choices form-select"></asp:DropDownList>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Second ID</label>
                            <asp:DropDownList runat="server" ID="ddlSecondId" CssClass="choices form-select"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcess"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcess" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Alias</h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDelete_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDeleteColour" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Colour Alias</h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdDeleteColour" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteColour" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteColour_Click" />
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

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblType"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>

    <asp:HiddenField ID="selected_tab" runat="server" />

    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            const gridConfigs = [
                { id: '<%= gvList.ClientID %>', link: "linkDetail" },
                { id: '<%= gvListColour.ClientID %>', link: "linkDetail" }
            ];

            gridConfigs.forEach(cfg => {
                const gv = document.getElementById(cfg.id);
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

                        const btn = this.querySelector(`a[id*='${cfg.link}']`);
                        if (btn) btn.click();
                    });
                }
            });
        });

        $(document).ready(function () {
            var selectedTab = $("#<%=selected_tab.ClientID%>");
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "list-fabric";
            $('#dvTab a[href="#' + tabId + '"]').tab('show');
            $("#dvTab a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });

            $("#listFabric").on("click", function () {
                updateSessionValue("list-fabric");
            });
            $("#listFabricColour").on("click", function () {
                updateSessionValue("list-fabriccolour");
            });
        });

        function updateSessionValue(session) {
            $.ajax({
                type: "POST",
                url: "Alias.aspx/UpdateSession",
                data: JSON.stringify({ value: session }),
                contentType: "application/json; charset=utf-8",
                dataType: "json"
            });
        }

        function showProcess() {
            $("#modalProcess").modal("show");
        }

        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }

        function showDeleteColour(id) {
            document.getElementById("<%=txtIdDeleteColour.ClientID %>").value = id;
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

        ["modalProcess", "modalDelete", "modalDeleteColour", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>