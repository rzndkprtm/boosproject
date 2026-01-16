<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Price_Promo_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Price Promo Detail" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price">Price</a></li>
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/price/promo">Promo</a></li>
                            <li class="breadcrumb-item active" aria-current="page"><%: Page.Title %></li>
                        </ol>
                    </nav>
                </div>
            </div>
        </div>
    </div>

    <div class="page-content">
        <section class="row" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>

        <section class="row mb-3">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <a href="#" class="btn btn-secondary" data-bs-toggle="modal" data-bs-target="#modalProcess">Log</a>
                <a href="#" runat="server" id="aEdit" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalProcess">Edit Promo</a>
            </div>
        </section>

        <section class="row">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12 col-sm-12 col-lg-6">
                                    <label>Promo Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblName" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-3">
                                    <label>Start Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblStartDate" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-3">
                                    <label>End Date</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblEndDate" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-6">
                                    <label>Description</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblDescription" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-3">
                                    <label>Active</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblActive" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-12 col-sm-12 col-lg-3">
                                    <label>ID</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblId" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
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
                                    <h3 class="card-title">Detail Promo</h3>
                                </div>
                                <div class="col-6 d-flex justify-content-end">
                                    <asp:Button runat="server" ID="btnAddDetail" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddDetail_Click" />
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
                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="Type" HeaderText="Type" />
                                                <asp:TemplateField HeaderText="Discount Tile">
                                                    <ItemTemplate>
                                                        <%# DiscountTitle(Eval("Type").ToString(), Eval("DataId").ToString()) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Discount">
                                                    <ItemTemplate>
                                                        <%# DiscountValue(Eval("Discount")) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Actions</button>
                                                        <ul class="dropdown-menu">
                                                            <li>
                                                                <asp:LinkButton runat="server" ID="linkDetailPromo" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteDetail" onclick='<%# String.Format("return showDeleteDetail(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                            </li>
                                                            <li>
                                                                <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkLogPromo" Text="Log" CommandName="Log" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
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

    <div class="modal fade text-left" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title">Edit Promo</h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-6 form-group">
                            <label class="form-label">Start Date</label>
                            <asp:TextBox runat="server" ID="txtStartDate" TextMode="Date" CssClass="form-control" placeholder="Start Date" autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">End Date</label>
                            <asp:TextBox runat="server" ID="txtEndDate" TextMode="Date" CssClass="form-control" placeholder="End Date" autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtDescription" Height="100px" CssClass="form-control" placeholder="Description ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-3 form-group">
                            <label class="form-label">Active</label>
                            <asp:DropDownList runat="server" ID="ddlActive" CssClass="form-select">
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

    <div class="modal fade text-left" id="modalProcessDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" runat="server" id="titleProcessDetail"></h4>
                </div>

                <div class="modal-body">
                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Type</label>
                            <asp:DropDownList runat="server" ID="ddlPromoType" CssClass="form-select" ClientIDMode="Static" onchange="visiblePromoType()">
                                <asp:ListItem Value="Designs" Text="Design Type"></asp:ListItem>
                                <asp:ListItem Value="Blinds" Text="Blind Type"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2" id="divDesigns">
                        <div class="col-12 form-group">
                            <label class="form-label">Product</label>
                            <asp:DropDownList runat="server" ID="ddlDesignPromo" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2" id="divBlinds">
                        <div class="col-12 form-group">
                            <label class="form-label">Product</label>
                            <asp:DropDownList runat="server" ID="ddlBlindPromo" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 form-group">
                            <label class="form-label">Discount</label>
                            <div class="input-group">
                                <asp:TextBox runat="server" ID="txtDiscount" CssClass="form-control" placeholder="Discount ..." autocomplete="off"></asp:TextBox>
                                <span class="input-group-text">%</span>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2" runat="server" id="divErrorProcessDetail">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessDetail"></span>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessDetail" CssClass="btn btn-primary" Text="Submit" OnClick="btnProcessDetail_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDeleteDetail" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Promo Detail</h5>
                </div>

                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtIdDeleteDetail" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteDetail" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteDetail_Click" />
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
                        <asp:GridView runat="server" ID="gvListLogs" CssClass="table table-vcenter card-table" AutoGenerateColumns="false" EmptyDataText="DATA NOT FOUND" EmptyDataRowStyle-HorizontalAlign="Center" ShowHeader="false" GridLines="None" BorderStyle="None">
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
        $(document).ready(function () {
            visiblePromoType();
        });
        function showProcess() {
            $("#modalProcess").modal("show");
        }
        function showProcessDetail() {
            $("#modalProcessDetail").modal("show");
        }
        function showDeleteDetail(id) {
            document.getElementById("<%=txtIdDeleteDetail.ClientID %>").value = id;
        }
        function showLog() {
            $("#modalLog").modal("show");
        }

        function visiblePromoType() {
            var type = document.getElementById("ddlPromoType");
            var divDesigns = document.getElementById("divDesigns");
            var divBlinds = document.getElementById("divBlinds");

            if (type.value === "Designs") {
                if (divDesigns) divDesigns.style.display = "block";
                if (divBlinds) divBlinds.style.display = "none";
            } else if (type.value === "Blinds") {
                if (divDesigns) divDesigns.style.display = "none";
                if (divBlinds) divBlinds.style.display = "block";
            } else {
                if (divDesigns) divDesigns.style.display = "none";
                if (divBlinds) divBlinds.style.display = "none";
            }
        }

        ["modalProcess", "modalProcessDetail", "modalDeleteDetail", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
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

                    const btn = this.querySelector("a[id*='linkDetailPromo']");
                    if (btn) btn.click();
                });
            }
        });

        window.history.replaceState(null, null, window.location.href);
    </script>


    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblIdDetail"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>