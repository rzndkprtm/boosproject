<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Address.aspx.vb" Inherits="Setting_Customer_Address" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Address" %>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer">Customer</a></li>
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
                        <div class="card-header">
                            <div class="row">
                                <div class="col-12 col-sm-12 col-lg-6 mb-2">
                                    <asp:Button runat="server" ID="btnAdd" CssClass="btn btn-primary" Text="Add New" OnClick="btnAdd_Click" />
                                </div>
                                <div class="col-12 col-sm-12 col-lg-6 d-flex justify-content-end">
                                    <asp:Panel runat="server" DefaultButton="btnSearch" Width="100%">
                                        <div class="input-group">
                                            <span class="input-group-text">Search : </span>
                                            <asp:TextBox runat="server" ID="txtSearch" CssClass="form-control" placeholoder=""></asp:TextBox>
                                            <asp:Button runat="server" ID="btnSearch" CssClass="btn btn-primary" Text="Search" OnClick="btnSearch_Click" />
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>

                        <div class="card-body">
                            <div class="row mb-2" runat="server" id="divError">
                                <div class="col-12">
                                    <div class="alert alert-danger">
                                        <span runat="server" id="msgError"></span>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-12">
                                    <div class="table-responsive">
                                        <asp:GridView runat="server" ID="gvList" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" PageSize="50" EmptyDataRowStyle-HorizontalAlign="Center" PagerSettings-Position="TopAndBottom" OnPageIndexChanging="gvList_PageIndexChanging" OnRowCommand="gvList_RowCommand">
                                            <RowStyle />
                                            <Columns>
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# Container.DataItemIndex + 1 %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="Id" HeaderText="ID" />
                                                <asp:BoundField DataField="CustomerName" HeaderText="Customer" />
                                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                                <asp:TemplateField HeaderText="Address">
                                                    <ItemTemplate>
                                                        <%# BindDetailAddress(Eval("Id").ToString()) %>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Note" HeaderText="Note" />
                                                <asp:BoundField DataField="DataPrimary" HeaderText="Primary" />
                                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                    <ItemTemplate>
                                                        <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                        <ul class="dropdown-menu">
                                                            <li runat="server" visible='<%# PageAction("Detail") %>'>
                                                                <asp:LinkButton runat="server" ID="linkDetail" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                            </li>
                                                            <li runat="server" visible='<%# PageAction("Delete") %>'>
                                                                <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDelete" onclick='<%# String.Format("return showDelete(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
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
                    
                    <div class="card-footer d-flex justify-content-between"></div>
                </div>
            </div>
        </section>
    </div>

    <div class="modal modal-blur fade" id="modalProcess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleProcess"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
            
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorProcess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcess"></span>
                            </div>
                        </div>
                    </div>
                    <div class="mb-3 row">
                        <div class="col-12">
                            <label class="form-label">Customer Account</label>
                            <asp:DropDownList runat="server" ID="ddlCustomer" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-12 col-sm-12 col-lg-6">
                            <label class="form-label">Description</label>
                            <asp:TextBox runat="server" ID="txtDescription" CssClass="form-control" placeholder="Description ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6">
                            <label class="form-label">Address</label>
                            <asp:TextBox runat="server" ID="txtAddress" CssClass="form-control" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                
                    <div class="mb-2 row">
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Suburb</label>
                            <asp:TextBox runat="server" ID="txtSuburb" CssClass="form-control" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">State</label>
                            <asp:TextBox runat="server" ID="txtState" CssClass="form-control" placeholder="State ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                
                    <div class="mb-2 row">
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Post Code</label>
                            <asp:TextBox runat="server" ID="txtPostCode" CssClass="form-control" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 form-group">
                            <label class="form-label">Country</label>
                            <asp:DropDownList runat="server" ID="ddlCountry" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Australia" Text="Australia"></asp:ListItem>
                                <asp:ListItem Value="Indonesia" Text="Indonesia"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Note</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtNote" CssClass="form-control" Height="100px" placeholder="Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Tags</label>
                            <asp:ListBox runat="server" ID="lbTags" CssClass="choices form-select multiple-remove" SelectionMode="Multiple">
                                <asp:ListItem Value="Delivery" Text="Delivery"></asp:ListItem>
                                <asp:ListItem Value="Warehouse" Text="Warehouse"></asp:ListItem>
                            </asp:ListBox>
                        </div>
                    </div>
                </div>
            
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcess" Text="Submit" CssClass="btn btn-primary" OnClick="btnProcess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Address</h5>
                </div>

                <div class="modal-body text-center py-4">
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

    <div class="modal modal-blur fade" id="modalLog" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
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
        document.addEventListener('DOMContentLoaded', function () {
            const gv = document.getElementById('<%= gvList.ClientID %>');
            if (!gv) return;

            for (let i = 1; i < gv.rows.length; i++) {
                const row = gv.rows[i];
                row.style.cursor = 'pointer';

                row.addEventListener('click', function (e) {
                    if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                        return;
                    }

                    const btn = this.querySelector("a[id*='linkDetail']");
                    if (btn) btn.click();
                });
            }
        });

        function showProcess() {
            $("#modalProcess").modal("show");
        }
        function showDelete(id) {
            document.getElementById("<%=txtIdDelete.ClientID %>").value = id;
        }
        function showLog() {
            $("#modalLog").modal("show");
        }
        ["modalProcess", "modalDelete", "modalLog"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblId"></asp:Label>
        <asp:Label runat="server" ID="lblAction"></asp:Label>
    </div>
</asp:Content>