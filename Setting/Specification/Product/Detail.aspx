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
                                <div class="col-6">
                                    <label>Product Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblName" CssClass="form-label font-bold"></asp:Label>
                                </div>

                                <div class="col-6">
                                    <label>Invoice Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblInvoiceName" CssClass="form-label font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Design Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblDesignName" CssClass="form-label font-bold"></asp:Label>
                                </div>

                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Blind Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblBlindName" CssClass="form-label font-bold"></asp:Label>
                                </div>

                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Company Detail</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCompanyName" CssClass="form-label font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Tube Type</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblTubeType" CssClass="form-label font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Control Type</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblControlType" CssClass="form-label font-bold"></asp:Label>
                                </div>
                                
                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Colour Type</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblColourType" CssClass="form-label font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>ID</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblId" CssClass="form-label font-bold"></asp:Label>
                                </div>

                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Active</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblActive" CssClass="form-label font-bold"></asp:Label>
                                </div>

                                <div class="col-12 col-sm-12 col-lg-4 mb-3">
                                    <label>Description</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblDescription" CssClass="form-label font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>

                        <div class="card-footer">
                            <asp:Button runat="server" ID="btnEditProduct" CssClass="btn btn-sm btn-primary" Text="Edit Product" OnClick="btnEditProduct_Click" />
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
                                <div class="col-12 col-sm-12 col-lg-6">
                                    <h4 class="card-title">List Hardware / Ven Kit</h4>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                    <asp:Button runat="server" ID="btnAddKit" CssClass="btn btn-primary btn-sm" Text="Add New" OnClick="btnAddKit_Click" />
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Id" HeaderStyle-CssClass="hiddencol" ItemStyle-CssClass="hiddencol" />
                                                <asp:BoundField DataField="KitId" HeaderText="KIT ID" />
                                                <asp:BoundField DataField="VenId" HeaderText="VEN ID" />
                                                <asp:BoundField DataField="Name" HeaderText="Name" />
                                                <asp:BoundField DataField="BlindStatus" HeaderText="Blind Status" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="200px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li runat="server" visible='<%# PageAction("Detail Kit") %>'>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Delete Kit") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteKit" onclick='<%# String.Format("return showDeleteKit(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton runat="server" ID="linkLog" CssClass="dropdown-item" Text="Log" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
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
        </section>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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

        function showLog() {
            $("#modalLog").modal("show");
        }

        ["modalProcessKit", "modalDeleteKit", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblIdKit"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>
