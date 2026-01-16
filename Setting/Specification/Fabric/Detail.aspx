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
                <div class="row mb-2" runat="server" id="divError">
                    <div class="col-12">
                        <div class="alert alert-danger">
                            <span runat="server" id="msgError"></span>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Fabric Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblName" CssClass="form-label font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Type</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblType" CssClass="form-label font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Group</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblGroup" CssClass="form-label font-bold"></asp:Label>
                                </div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Design Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblDesignName" CssClass="form-label font-bold"></asp:Label>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Tube Type</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblTubeType" CssClass="form-label font-bold"></asp:Label>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Company Detail</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCompanyDetailName" CssClass="form-label font-bold"></asp:Label>
                                </div>
                            </div>
                            <div class="row mb-3">
                                <div class="col-4">
                                    <label>ID</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblId" CssClass="form-label font-bold"></asp:Label>
                                </div>
                                <div class="col-4">
                                    <label>No Rail Road</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblNoRailRoad" CssClass="form-label font-bold"></asp:Label>
                                </div>
                                <div class="col-4">
                                    <label>Active</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblActive" CssClass="form-label font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer">
                            <a href="#" runat="server" id="aEditFabric" class="btn btn-sm btn-primary" data-bs-toggle="modal" data-bs-target="#modalEdit">Edit Fabric</a>
                            <a href="#" runat="server" id="aDeleteFabric" class="btn btn-sm btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete Fabric</a>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-header">
                            <div class="row">
                                <div class="col-6">
                                    <h3 class="card-title">List Fabric Colour</h3>
                                </div>
                                <div class="col-6 d-flex justify-content-end">
                                    <asp:Button runat="server" ID="btnAddColour" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddColour_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="True" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                                <asp:BoundField DataField="BoeId" HeaderText="BOE ID" />
                                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                                <asp:BoundField DataField="Colour" HeaderText="Colour" />
                                                <asp:BoundField DataField="Width" HeaderText="Width" />
                                                <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="150px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li runat="server" visible='<%# PageAction("Detail Colour") %>'>
                                                                <asp:LinkButton runat="server" ID="linkDetailColour" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Delete Colour") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteColour" onclick='<%# String.Format("return showDeleteColour(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton runat="server" ID="linkLog" CssClass="dropdown-item" Text="Log" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                        </ul>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <PagerStyle BackColor="DodgerBlue" ForeColor="White" HorizontalAlign="Center" />
                                            <PagerSettings PreviousPageText="Prev" NextPageText="Next" Mode="NumericFirstLast" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal fade text-left" id="modalEdit" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Edit Fabric</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorEdit">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorEdit"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Fabric Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Design Name</label>
                            <asp:ListBox runat="server" ID="lbDesign" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Tube Name</label>
                            <asp:ListBox runat="server" ID="lbTube" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Company Detail</label>
                            <asp:ListBox runat="server" ID="lbCompany" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-4 form-group">
                            <label class="form-label">Type</label>
                            <asp:DropDownList runat="server" ID="ddlType" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Blockout" Text="Blockout"></asp:ListItem>
                                <asp:ListItem Value="Light Filtering" Text="Light Filtering"></asp:ListItem>
                                <asp:ListItem Value="Screen" Text="Screen"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-5 form-group">
                            <label class="form-label">Group</label>
                            <asp:DropDownList runat="server" ID="ddlGroup" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Group 1" Text="Group 1"></asp:ListItem>
                                <asp:ListItem Value="Group 2" Text="Group 2"></asp:ListItem>
                                <asp:ListItem Value="Group 3" Text="Group 3"></asp:ListItem>
                                <asp:ListItem Value="Group 4" Text="Group 4"></asp:ListItem>
                                <asp:ListItem Value="Group Express" Text="Group Express"></asp:ListItem>
                                <asp:ListItem Value="Opaque" Text="Opaque"></asp:ListItem>
                                <asp:ListItem Value="Semi Opaque" Text="Semi Opaque"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-3 form-group">
                            <label class="form-label">No Rail Road</label>
                            <asp:DropDownList runat="server" ID="ddlNoRailRoad" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-3 form-group">
                            <label class="form-label">Active</label>
                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnEdit" CssClass="btn btn-primary" Text="Submit" OnClick="btnEdit_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-left" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
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
                    
                    <div class="row mb-2">
                        <div class="col-3 form-group">
                            <label class="form-label">Active</label>
                            <asp:DropDownList runat="server" ID="ddlActiveColour" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
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
                    <asp:Button runat="server" ID="btnProcess" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Fabric Type</h5>
                </div>

                <div class="modal-body">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDelete" CssClass="btn btn-danger" Text="Submit" OnClick="btnDelete_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDeleteColour" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Fabric Colour</h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdDeleteColour" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteColour" CssClass="btn btn-danger" Text="Submit" OnClick="btnDeleteColour_Click" />
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
                    <div class="row" runat="server" id="divErrorLog">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorLog"></span>
                            </div>
                        </div>
                    </div>
                    <div class="table-responsive">
                        <asp:GridView runat="server" ID="gvListLogs" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA LOG NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeader="false" GridLines="None" BorderStyle="None">
                            <RowStyle />
                            <Columns>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <%# BindTextLog(Eval("Id").ToString()) %>
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

    <script type="text/javascript">
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

                    const btn = this.querySelector("a[id*='linkDetailColour']");
                    if (btn) btn.click();
                });
            }
        });
        function showProcess() {
            $("#modalProcess").modal("show");
        }
        function showDeleteColour(id) {
            document.getElementById("<%=txtIdDeleteColour.ClientID %>").value = id;
        }
        function showLog() {
            $("#modalLog").modal("show");
        }
        ["modalEdit", "modalProcess", "modalDelete", "modalDeleteColour", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblIdColour"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>