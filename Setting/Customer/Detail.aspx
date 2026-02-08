<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Customer_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Detail" %>

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
        <section class="row" runat="server" id="divError">
            <div class="col-12">
                <div class="alert alert-danger">
                    <span runat="server" id="msgError"></span>
                </div>
            </div>
        </section>

        <section class="row mb-3">
            <div class="col-lg-12 d-flex flex-wrap justify-content-end gap-1">
                <asp:Button runat="server" ID="btnEditCustomer" CssClass="btn btn-primary" Text="Edit" OnClick="btnEditCustomer_Click" />
                <a href="#" runat="server" id="aDelete" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                <a href="#" runat="server" id="aCreateOrder" class="btn btn-info" data-bs-toggle="modal" data-bs-target="#modalCreateOrder">Create Order</a>
                <a href="javascript:void(0)" class="btn btn-secondary" onclick="showLog('Customers', '<%= lblId.Text %>')">Log</a>
            </div>
        </section>

        <section class="row">            
            <div class="col-12 col-sm-12 col-lg-9">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="row mb-3">
                                <div class="col-12">
                                    <label>Customer Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblName" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>Company</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCompanyId" Visible="false"></asp:Label>
                                    <asp:Label runat="server" ID="lblCompanyName" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>Sub Company</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCompanyDetailName" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>Area</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblArea" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>Operator</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOperator" CssClass="font-bold"></asp:Label>
                                </div>                                
                            </div>

                            <div class="row mb-3" runat="server" id="divLevelSponsor">
                                <div class="col-6 col-sm-6 col-lg-5 mb-3">
                                    <label>Level</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblLevel" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-7 mb-3">
                                    <label>Sponsor Name</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblSponsor" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Price Group</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblPriceGroup" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Shutter Price Group</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblPriceGroupShutter" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-4 mb-3">
                                    <label>Door Price Group</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblPriceGroupDoor" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>On Stop</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblOnStop" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>Cash Sale</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblCashSale" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>Newsletter</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblNewsletter" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-6 col-sm-6 col-lg-3 mb-3">
                                    <label>Mininimum Surcharge</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblMinSurcharge" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>

                            <div class="row mb-3">
                                <div class="col-4 col-sm-4 col-lg-4">
                                    <label>ID</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblId" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-4 col-sm-4 col-lg-4">
                                    <label>Debtor Code</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblDebtorCode" CssClass="font-bold"></asp:Label>
                                </div>
                                <div class="col-4 col-sm-4 col-lg-4">
                                    <label>Active</label>
                                    <br />
                                    <asp:Label runat="server" ID="lblActive" CssClass="font-bold"></asp:Label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>

        <section class="row" runat="server" id="secDetail">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                <a class="list-group-item list-group-item-action active" id="listContact" data-bs-toggle="list" href="#list-contact" role="tab">Contact</a>
                                <a class="list-group-item list-group-item-action" id="listAddress" data-bs-toggle="list" href="#list-address" role="tab">Address</a>
                                <a class="list-group-item list-group-item-action" id="listBusiness" data-bs-toggle="list" href="#list-business" role="tab">Business</a>
                                <a class="list-group-item list-group-item-action" id="listLogin" data-bs-toggle="list" href="#list-login" role="tab">Login</a>
                                <a class="list-group-item list-group-item-action" id="listDiscount" data-bs-toggle="list" href="#list-discount" role="tab">Discount</a>
                                <a class="list-group-item list-group-item-action" id="listPromo" data-bs-toggle="list" href="#list-promo" role="tab">Promo</a>
                                <a class="list-group-item list-group-item-action" id="listProduct" data-bs-toggle="list" href="#list-product" role="tab">Product Access</a>
                                <a class="list-group-item list-group-item-action" id="listQuote" data-bs-toggle="list" href="#list-quote" role="tab">Quote</a>
                            </div>

                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-contact" role="tabpanel" aria-labelledby="listContact">
                                    <div class="row mt-5" runat="server" id="divErrorContact">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorContact"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListContact" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListContact_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="ContactName" HeaderText="Name" />
                                                        <asp:BoundField DataField="Role" HeaderText="Role" />
                                                        <asp:BoundField DataField="Email" HeaderText="Email" />
                                                        <asp:BoundField DataField="Tags" HeaderText="Tags" />
                                                        <asp:TemplateField HeaderText="Primary">
                                                            <ItemTemplate>
                                                                <i runat="server" visible='<%# VisibleYesPrimaryContact(Eval("Primary")) %>' class="bi bi-check-circle-fill"></i>
                                                                <i runat="server" visible='<%# VisibleNoPrimaryContact(Eval("Primary")) %>' class="bi bi-x-circle-fill"></i>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail Contact") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetailContact" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Delete Contact") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteContact" onclick='<%# String.Format("return showDeleteContact(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# VisiblePrimaryContact(Eval("Primary")) %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPrimaryContact" onclick='<%# String.Format("return showPrimaryContact(`{0}`);", Eval("Id").ToString()) %>'>Set As Primary</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('CustomerContacts', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <asp:Button runat="server" ID="btnAddContact" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddContact_Click" />
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-address" role="tabpanel" aria-labelledby="listAddress">
                                    <div class="row mt-5">
                                        <div class="col-12" runat="server" id="divErrorAddress">
                                             <div class="col-12">
                                                 <div class="alert alert-danger">
                                                     <span runat="server" id="msgErrorAddress"></span>
                                                 </div>
                                             </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListAddress" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="ADDRESS NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListAddress_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                                        <asp:TemplateField HeaderText="Address">
                                                            <ItemTemplate>
                                                                <%# BindDetailAddress(Eval("Id").ToString()) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Note" HeaderText="Note" />
                                                        <asp:TemplateField HeaderText="Primary">
                                                            <ItemTemplate>
                                                                <i runat="server" visible='<%# VisibleYesPrimaryAddress(Eval("Primary")) %>' class="bi bi-check-circle-fill"></i>
                                                                <i runat="server" visible='<%# VisibleNoPrimaryAddress(Eval("Primary")) %>' class="bi bi-x-circle-fill"></i>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail Address") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetailAddress" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Delete Address") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteAddress" onclick='<%# String.Format("return showDeleteAddress(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# VisiblePrimaryAddress(Eval("Primary")) %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPrimaryAddress" onclick='<%# String.Format("return showPrimaryAddress(`{0}`);", Eval("Id").ToString()) %>'>Set As Primary</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('CustomerAddress', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div> 
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <asp:Button runat="server" ID="btnAddAddress" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddAddress_Click" />
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-business" role="tabpanel" aria-labelledby="listBusiness">
                                    <div class="row mt-5">
                                        <div class="col-12" runat="server" id="divErrorBusiness">
                                             <div class="col-12">
                                                 <div class="alert alert-danger">
                                                     <span runat="server" id="msgErrorBusiness"></span>
                                                 </div>
                                             </div>
                                        </div>

                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="table-responsive">
                                                    <asp:GridView runat="server" ID="gvListBusiness" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListBusiness_RowCommand">
                                                        <RowStyle />
                                                        <Columns>
                                                            <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                                <ItemTemplate>
                                                                    <%# Container.DataItemIndex + 1 %>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:BoundField DataField="Id" HeaderText="ID" />
                                                            <asp:BoundField DataField="ABNNumber" HeaderText="ABN Number" />
                                                            <asp:BoundField DataField="RegisteredName" HeaderText="Registered Name" />
                                                            <asp:BoundField DataField="RegisteredDate" HeaderText="Registered Date" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:BoundField DataField="ExpiryDate" HeaderText="Expiry Date" DataFormatString="{0:dd MMM yyyy}" />
                                                            <asp:TemplateField HeaderText="Primary">
                                                                <ItemTemplate>
                                                                    <i runat="server" visible='<%# VisibleYesPrimaryBusiness(Eval("Primary")) %>' class="bi bi-check-circle-fill"></i>
                                                                    <i runat="server" visible='<%# VisibleNoPrimaryBusiness(Eval("Primary")) %>' class="bi bi-x-circle-fill"></i>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                            <asp:TemplateField ItemStyle-Width="120px">
                                                                <ItemTemplate>
                                                                    <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                    <ul class="dropdown-menu">
                                                                        <li runat="server" visible='<%# PageAction("Detail Business") %>'>
                                                                            <asp:LinkButton runat="server" ID="linkDetailBusiness" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                        </li>
                                                                        <li runat="server" visible='<%# PageAction("Delete Business") %>'>
                                                                            <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteBusiness" onclick='<%# String.Format("return showDeleteBusiness(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                        </li>
                                                                        <li runat="server" visible='<%# VisiblePrimaryBusiness(Eval("Primary")) %>'>
                                                                            <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPrimaryBusiness" onclick='<%# String.Format("return showPrimaryBusiness(`{0}`);", Eval("Id").ToString()) %>'>Set As Primary</a>
                                                                        </li>
                                                                        <li>
                                                                            <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('CustomerBusiness', '<%# Eval("Id") %>')">Log</a>
                                                                        </li>
                                                                    </ul>
                                                                </ItemTemplate>
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div> 
                                        </div>

                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <asp:Button runat="server" ID="btnAddBusiness" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddBusiness_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-login" role="tabpanel" aria-labelledby="listLogin">
                                    <div class="row mt-5">
                                        <div class="col-12" runat="server" id="divErrorLogin">
                                             <div class="col-12">
                                                 <div class="alert alert-danger">
                                                     <span runat="server" id="msgErrorLogin"></span>
                                                 </div>
                                             </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListLogin" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="LOGIN NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListLogin_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:TemplateField HeaderText="Role">
                                                            <ItemTemplate>
                                                                <%# If(IsDBNull(Eval("RoleName")) OrElse IsDBNull(Eval("LevelName")) OrElse String.IsNullOrEmpty(Eval("RoleName") & "") OrElse String.IsNullOrEmpty(Eval("LevelName") & ""), "Requires correction", Eval("RoleName") & " - " & Eval("LevelName")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="UserName" HeaderText="User" />
                                                        <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                                                        <asp:BoundField DataField="Email" HeaderText="Email" />
                                                        <asp:BoundField DataField="LastLogin" HeaderText="Last Login" DataFormatString="{0:dd MMM yyyy HH:mm:ss}" />
                                                        <asp:BoundField DataField="DataActive" HeaderText="Active" />
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail Login") %>'>
                                                                        <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkDetailLogin" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# VisibleInstallerAccess(Eval("RoleId").ToString()) %>'>
                                                                        <asp:LinkButton runat="server" CssClass="dropdown-item" ID="linkInstallerAccess" Text="Installer Access" CommandName="InstallerAccess" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Active Login") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalActiveLogin" onclick='<%# String.Format("return showActiveLogin(`{0}`, `{1}`);", Eval("Id").ToString(), Convert.ToInt32(Eval("Active"))) %>'><%# TextActive_Login(Eval("Active")) %></a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Reset Login") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalResetPass" onclick='<%# String.Format("return showResetPass(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("UserName").ToString()) %>'>Reset Password</a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Dencrypt Login") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDencryptPass" onclick='<%# String.Format("return showDencryptPass(`{0}`, `{1}`);", Eval("UserName").ToString(), DencryptPassword(Eval("Password").ToString())) %>'>Show Password</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('CustomerLogins', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div> 
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <asp:Button runat="server" ID="btnAddLogin" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddLogin_Click" />
                                            <a href="javascript:void(0)" runat="server" id="aCredentialsLogin" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalCredentialsLogin">Send Login Credentials</a>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-discount" role="tabpanel" aria-labelledby="listDiscount">
                                    <div class="row mt-5">
                                        <div class="col-12" runat="server" id="divErrorDiscount">
                                             <div class="col-12">
                                                 <div class="alert alert-danger">
                                                     <span runat="server" id="msgErrorDiscount"></span>
                                                 </div>
                                             </div>
                                        </div>
                                    </div>
                                    
                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListDiscount" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListDiscount_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
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
                                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail Discount") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetailDiscount" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Delete Discount") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteDiscount" onclick='<%# String.Format("return showDeleteDiscount(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('CustomerDiscounts', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <asp:Button runat="server" ID="btnAddDiscount" CssClass="btn btn-primary" Text="Add Discount (All Products)" OnClick="btnAddDiscount_Click" />

                                            <asp:Button runat="server" ID="btnAddDiscountCustom" CssClass="btn btn-info" Text="Add Discount (Custom Product)" OnClick="btnAddDiscountCustom_Click" />

                                            <a href="#" runat="server" id="aResetDiscount" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalResetDiscount">Reset Discount</a>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-promo" role="tabpanel" aria-labelledby="listPromo">
                                    <div class="row mt-5">
                                        <div class="col-12" runat="server" id="divErrorPromo">
                                             <div class="col-12">
                                                 <div class="alert alert-danger">
                                                     <span runat="server" id="msgErrorPromo"></span>
                                                 </div>
                                             </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListPromo" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListPromo_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:BoundField DataField="PromoName" HeaderText="Promo" />
                                                         <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:dd MMM yyyy}" />
                                                        <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:dd MMM yyyy}" />
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail Promo") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetailPromo" CssClass="dropdown-item" Text="Detail" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li runat="server" visible='<%# PageAction("Delete Promo") %>'>
                                                                        <a href="#" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeletePromo" onclick='<%# String.Format("return showDeletePromo(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('CustomerPromos', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <asp:Button runat="server" ID="btnAddPromo" CssClass="btn btn-primary" Text="Add Promo" OnClick="btnAddPromo_Click" />
                                            <a href="#" runat="server" id="aResetPromo" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalResetPromo">Reset Promo</a>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-product" role="tabpanel" aria-labelledby="listProduct">
                                    <div class="row mt-5">
                                        <div class="col-12" runat="server" id="divErrorProduct">
                                             <div class="col-12">
                                                 <div class="alert alert-danger">
                                                     <span runat="server" id="msgErrorProduct"></span>
                                                 </div>
                                             </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListProduct" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowCommand="gvListProduct_RowCommand">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                                        <asp:TemplateField HeaderText="Product" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# BindDetailProduct(Eval("Id").ToString()) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li runat="server" visible='<%# PageAction("Detail Product Access") %>'>
                                                                        <asp:LinkButton runat="server" ID="linkDetailProduct" CssClass="dropdown-item" Text="Detail / Edit" CommandName="Detail" CommandArgument='<%# Eval("Id") %>'></asp:LinkButton>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0)" class="dropdown-item" onclick="showLog('CustomerProductAccess', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <a href="#" runat="server" id="aResetProduct" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalResetProduct">Reset Product Access</a>
                                        </div>
                                    </div>
                                </div>

                                <div class="tab-pane fade" id="list-quote" role="tabpanel" aria-labelledby="listQuote">
                                    <div class="row mt-5">
                                        <div class="col-12" runat="server" id="divErrorQuote">
                                            <div class="col-12">
                                                 <div class="alert alert-danger">
                                                     <span runat="server" id="msgErrorQuote"></span>
                                                 </div>
                                             </div>
                                        </div>
                                    </div>

                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListQuote" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" AllowPaging="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" PageSize="50" PagerSettings-Position="TopAndBottom">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Email" HeaderText="Email" />
                                                        <asp:BoundField DataField="Phone" HeaderText="Phone" />
                                                        <asp:TemplateField HeaderText="Address">
                                                            <ItemTemplate>
                                                                <%# BindQuoteddress(Eval("Id").ToString()) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Logo">
                                                            <ItemTemplate>
                                                                <asp:Image runat="server" ImageUrl='<%# ResolveUrl("~/assets/images/logo/customers/") & Eval("Logo") %>'  Width="220px" Height="70px" />
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Terms" HeaderText="Terms" />
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

    <asp:HiddenField ID="selected_tab" runat="server" />

    <div class="modal fade text-center" id="modalDelete" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Customer</h5>
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

    <div class="modal fade text-left" id="modalCreateOrder" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Create Order</h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorCreateOrder">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorCreateOrder"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Number</label>
                            <asp:TextBox runat="server" ID="txtOrderNumber" CssClass="form-control" placeholder="Order Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Name</label>
                            <asp:TextBox runat="server" ID="txtOrderName" CssClass="form-control" placeholder="Order Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Order Note</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtOrderNote" CssClass="form-control" Height="100px" placeholder="Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divOrderType">
                        <div class="col-4 form-group">
                            <label class="form-label">Order Type</label>
                            <asp:DropDownList runat="server" ID="ddlOrderType" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Regular" Text="Regular"></asp:ListItem>
                                <asp:ListItem Value="Builder" Text="Builder"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCreateOrder" Text="Submit" CssClass="btn btn-primary" OnClick="btnCreateOrder_Click" />
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

    <%--CUSTOMER CONTACT--%>
    <div class="modal fade text-left" id="modalProcessContact" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 runat="server" class="modal-title" id="titleContact"></h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorProcessContact">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessContact"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Name</label>
                            <asp:TextBox runat="server" ID="txtContactName" CssClass="form-control" placeholder="Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Salutation</label>
                            <asp:DropDownList runat="server" ID="ddlContactSalutation" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Mr." Text="Mr."></asp:ListItem>
                                <asp:ListItem Value="Mrs." Text="Mrs."></asp:ListItem>
                                <asp:ListItem Value="Ms." Text="Ms."></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Role</label>
                            <asp:TextBox runat="server" ID="txtContactRole" CssClass="form-control" placeholder="Role ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Email</label>
                            <asp:TextBox runat="server" ID="txtContactEmail" TextMode="Email" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Phone</label>
                            <asp:TextBox runat="server" ID="txtContactPhone" CssClass="form-control" placeholder="Phone ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Mobile</label>
                            <asp:TextBox runat="server" ID="txtContactMobile" CssClass="form-control" placeholder="Mobile ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">FAX</label>
                            <asp:TextBox runat="server" ID="txtContactFax" CssClass="form-control" placeholder="FAX ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Tags</label>
                            <asp:ListBox runat="server" ID="lbContactTags" CssClass="choices form-select multiple-remove" SelectionMode="Multiple">
                                 <asp:ListItem Value="Confirming" Text="Confirming"></asp:ListItem>
                                 <asp:ListItem Value="Invoicing" Text="Invoicing"></asp:ListItem>
                                 <asp:ListItem Value="Quoting" Text="Quoting"></asp:ListItem>
                                 <asp:ListItem Value="Newsletter" Text="Newsletter"></asp:ListItem>
                            </asp:ListBox>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Note</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtContactNote" CssClass="form-control" Height="100px" placeholder="Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessContact" Text="Submit" CssClass="btn btn-primary" OnClick="btnProcessContact_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeleteContact" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Customer Contact</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdContactDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteContact" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteContact_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalPrimaryContact" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-secondary">
                    <h5 class="modal-title white">Set Primary Contact</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdPrimaryContact" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnPrimaryContact" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnPrimaryContact_Click" />
                </div>
            </div>
        </div>
    </div>
    
    <%--CUSTOMER ADDRESS--%>
    <div class="modal modal-blur fade" id="modalProcessAddress" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleAddress"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorProcessAddress">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessAddress"></span>
                            </div>
                        </div>
                    </div>
                    <div class="mb-2 row">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Desciption</label>
                            <asp:TextBox runat="server" ID="txtAddressDescription" CssClass="form-control" placeholder="Desciption ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Address</label>
                            <asp:TextBox runat="server" ID="txtAddressName" CssClass="form-control" placeholder="Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-2 row">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Suburb</label>
                            <asp:TextBox runat="server" ID="txtAddressSuburb" CssClass="form-control" placeholder="Suburb ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">State</label>
                            <asp:TextBox runat="server" ID="txtAddressState" CssClass="form-control" placeholder="State ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="mb-2 row">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Post Code</label>
                            <asp:TextBox runat="server" ID="txtAddressPostCode" CssClass="form-control" placeholder="Post Code ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label">Country</label>
                            <asp:DropDownList runat="server" ID="ddlAddressCountry" CssClass="form-select">
                                <asp:ListItem Value="" Text=""></asp:ListItem>
                                <asp:ListItem Value="Australia" Text="Australia"></asp:ListItem>
                                <asp:ListItem Value="Indonesia" Text="Indonesia"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                    
                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Note</label>
                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtAddressNote" CssClass="form-control" Height="70px" placeholder="Note ..." autocomplete="off" style="resize:none;"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Tags</label>
                            <asp:ListBox runat="server" ID="lbAddressTags" CssClass="choices form-select multiple-remove" SelectionMode="Multiple">
                                <asp:ListItem Value="Delivery" Text="Delivery"></asp:ListItem>
                                <asp:ListItem Value="Warehouse" Text="Warehouse"></asp:ListItem>
                            </asp:ListBox>
                        </div>
                    </div>
                </div>
                
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessAddress" Text="Submit" CssClass="btn btn-primary" OnClick="btnProcessAddress_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeleteAddress" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Customer Address</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdAddressDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteAddress" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteAddress_Click" />
                </div>
            </div>
        </div>
    </div>
    
    <div class="modal modal-blur fade" id="modalPrimaryAddress" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-secondary">
                    <h5 class="modal-title white">Set Primary Address</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdPrimaryAddress" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnPrimaryAddress" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnPrimaryAddress_Click" />
                </div>
            </div>
        </div>
    </div>

    <%--CUSTOMER BUSINESS--%>
    <div class="modal modal-blur fade" id="modalProcessBusiness" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleBusiness"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                
                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorProcessBusiness">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessBusiness"></span>
                            </div>
                        </div>
                    </div>
                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">ABN Number</label>
                            <asp:TextBox runat="server" ID="txtBusinessNumber" CssClass="form-control" placeholder="ABN Number ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Registered Name</label>
                            <asp:TextBox runat="server" ID="txtBusinessName" CssClass="form-control" placeholder="Registered Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="mb-2 row">
                        <div class="col-6 form-group">
                            <label class="form-label">Registered Date</label>
                            <asp:TextBox runat="server" ID="txtBusinessRegistered" TextMode="Date" CssClass="form-control" placeholder="Registered Date ..." autocomplete="off"></asp:TextBox>
                        </div>
                        <div class="col-6 form-group">
                            <label class="form-label">Expiry Date</label>
                            <asp:TextBox runat="server" ID="txtBusinessExpiry" TextMode="Date" CssClass="form-control" placeholder="Expiry Date ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessBusiness" Text="Submit" CssClass="btn btn-primary" OnClick="btnProcessBusiness_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeleteBusiness" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Customer Business</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdBusinessDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteBusiness" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteBusiness_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalPrimaryBusiness" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-secondary">
                    <h5 class="modal-title white">Set Primary Business</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdPrimaryBusiness" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnPrimaryBusiness" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnPrimaryBusiness_Click" />
                </div>
            </div>
        </div>
    </div>

    <%--CUSTOMER LOGIN--%>
    <div class="modal modal-blur fade" id="modalProcessLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleLogin"></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorProcessLogin">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessLogin"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row mb-2">
                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label required">Role</label>
                            <asp:DropDownList runat="server" ID="ddlLoginRole" CssClass="form-select"></asp:DropDownList>
                        </div>

                        <div class="col-12 col-sm-12 col-lg-6 mb-2 form-group">
                            <label class="form-label required">Level</label>
                            <asp:DropDownList runat="server" ID="ddlLoginLevel" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label required">Full Name</label>
                            <asp:TextBox runat="server" ID="txtLoginFullName" CssClass="form-control" placeholder="Full Name ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divLoginEmail">
                        <div class="col-12 form-group">
                            <label class="form-label required">Email</label>
                            <asp:TextBox runat="server" ID="txtLoginEmail" CssClass="form-control" placeholder="Email ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label required">Username</label>
                            <asp:TextBox runat="server" ID="txtLoginUserName" CssClass="form-control" placeholder="UserName ..." autocomplete="new-password"></asp:TextBox>
                        </div>
                    </div>

                    <div class="row" runat="server" id="divPassword">
                        <div class="col-12 form-group">
                            <label class="form-label">Password</label>
                            <asp:TextBox runat="server" ID="txtLoginPassword" CssClass="form-control" placeholder="Password ..."></asp:TextBox>
                        </div>
                        <small class="form-hint" id="passwordinfo"></small>
                    </div>

                    <div class="row">
                        <div class="col-12 col-sm-12 col-lg-4 mb-2 form-group">
                            <label class="form-label">Pricing</label>
                            <asp:DropDownList runat="server" ID="ddlPricing" CssClass="form-select">
                                <asp:ListItem Value="1" Text="Yes"></asp:ListItem>
                                <asp:ListItem Value="0" Text="No"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessLogin" Text="Submit" CssClass="btn btn-primary" OnClick="btnProcessLogin_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalInstallerAccess" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Installer Access</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>

                <div class="modal-body">
                    <div class="row" runat="server" id="divErrorInstallerAccess">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorInstallerAccess"></span>
                            </div>
                        </div>
                    </div>
                    <div class="row mt-2 mb-5">
                        <div class="col-12 form-group">
                            <label class="form-label required">Customer Area</label>
                            <asp:ListBox runat="server" ID="lbInstallerAccess" CssClass="choices form-select multiple-remove" SelectionMode="Multiple">
                                <asp:ListItem Value="BP" Text="BP"></asp:ListItem>
                                <asp:ListItem Value="NSW 1" Text="NSW 1"></asp:ListItem>
                                <asp:ListItem Value="NSW 2" Text="NSW 2"></asp:ListItem>
                                <asp:ListItem Value="QLD" Text="QLD"></asp:ListItem>
                                <asp:ListItem Value="SA" Text="SA"></asp:ListItem>
                                <asp:ListItem Value="VIC 1" Text="VIC 1"></asp:ListItem>
                                <asp:ListItem Value="VIC 2" Text="VIC 2"></asp:ListItem>
                                <asp:ListItem Value="WA" Text="WA"></asp:ListItem>
                            </asp:ListBox>
                        </div>
                    </div>
                </div>
                
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnInstallerAccess" Text="Submit" CssClass="btn btn-primary" OnClick="btnInstallerAccess_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalActiveLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white" id="titleActiveLogin"></h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdActiveLogin" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtActiveLogin" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnActiveLogin" CssClass="btn btn-warning" Text="Confirm" OnClick="btnActiveLogin_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalResetPass" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Reset Password</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdResetPass" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtNewResetPass" style="display:none;"></asp:TextBox>
                    <span id="spanDescResetPass"></span>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnResetPass" CssClass="btn btn-info" Text="Confirm" OnClick="btnResetPass_Click" />
                </div>
            </div>
        </div>
    </div>    

    <div class="modal modal-blur fade" id="modalDencryptPass" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-warning">
                    <h5 class="modal-title white">Show Password</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <span id="spanPassword"></span>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalCredentialsLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Send Login Credentials</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />All login details will be sent to the primary contact email address.<br /><br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnCredentialsLogin" CssClass="btn btn-danger" Text="Confirm" OnClick="btnCredentialsLogin_Click" />
                </div>
            </div>
        </div>
    </div>

    <%--CUSTOMER DISCOUNT--%>
    <div class="modal modal-blur fade" id="modalProcessDiscount" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titleDiscount"></h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorProcessDiscount">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessDiscount"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row" id="divDiscountType">
                        <div class="col-12 form-group">
                            <label class="form-label required">Type</label>
                            <asp:DropDownList runat="server" ID="ddlDiscountType" CssClass="form-select" ClientIDMode="Static" onchange="visibleDiscountType()">
                                <asp:ListItem Value="Designs" Text="Design Type"></asp:ListItem>
                                <asp:ListItem Value="PriceProductGroups" Text="Product Group"></asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>

                    <div class="row" id="divDiscountDataId">
                        <div class="col-12 form-group">
                            <label class="form-label required">Product</label>
                            <asp:DropDownList runat="server" ID="ddlDiscountDataId" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row" id="divDiscountDataIdB">
                        <div class="col-12 form-group">
                            <label class="form-label required">Product</label>
                            <asp:DropDownList runat="server" ID="ddlDiscountDataIdB" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label required">Discount Value</label>
                            <div class="input-group">
                                <asp:TextBox runat="server" TextMode="Number" ID="txtDiscountValue" CssClass="form-control" placeholder="Discount ......" autocomplete="off"></asp:TextBox>
                                <span class="input-group-text">%</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessDiscount" CssClass="btn btn-success" Text="Submit" OnClick="btnProcessDiscount_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalResetDiscount" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Reset Discount</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnResetDiscount" CssClass="btn btn-danger" Text="Confirm" OnClick="btnResetDiscount_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeleteDiscount" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Customer Discount</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdDiscountDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteDiscount" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteDiscount_Click" />
                </div>
            </div>
        </div>
    </div>

    <%--CUSTOMER PROMO--%>
    <div class="modal modal-blur fade" id="modalProcessPromo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" runat="server" id="titlePromo"></h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorProcessPromo">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessPromo"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row" runat="server">
                        <div class="col-12 form-group">
                            <label class="form-label required">Promo</label>
                            <asp:DropDownList runat="server" ID="ddlListPromo" CssClass="form-select"></asp:DropDownList>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessPromo" CssClass="btn btn-success" Text="Submit" OnClick="btnProcessPromo_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDetailPromo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Detail Promo</h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorDetailPromo">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorDetailPromo"></span>
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-12">
                            <div class="table-responsive">
                                <asp:GridView runat="server" ID="gvListDetailPromo" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                    <RowStyle />
                                    <Columns>
                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                            <ItemTemplate>
                                                <%# Container.DataItemIndex + 1 %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Id" HeaderText="ID" />
                                        <asp:TemplateField HeaderText="Type">
                                            <ItemTemplate>
                                                <%# PromoTitle(Eval("Type").ToString(), Eval("DataId").ToString()) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Discount">
                                            <ItemTemplate>
                                                <%# PromoValue(Eval("Discount")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalResetPromo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Reset Promo</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnResetPromo" CssClass="btn btn-danger" Text="Confirm" OnClick="btnResetPromo_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalDeletePromo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Customer Promo</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtIdPromoDelete" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeletePromo" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeletePromo_Click" />
                </div>
            </div>
        </div>
    </div>

    <%--CUSTOMER PRODUCT ACCESS--%>
    
    <div class="modal fade text-left" id="modalProcessProduct" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Edit Product Access</h5>
                </div>
                <div class="modal-body">
                    <div class="row mb-2" runat="server" id="divErrorProcessProduct">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorProcessProduct"></span>
                            </div>
                        </div>
                    </div>

                    <div class="mb-3 row">
                        <div class="col-12 form-group">
                            <label class="form-label">Products</label>
                            <asp:ListBox runat="server" ID="lbProductTags" CssClass="choices form-select multiple-remove" SelectionMode="Multiple"></asp:ListBox>
                        </div>
                    </div>
                </div>

                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnProcessProduct" Text="Submit" CssClass="btn btn-primary" OnClick="btnProcessProduct_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-blur fade" id="modalResetProduct" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Reset Product Access</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="#" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitResetProduct" CssClass="btn btn-danger" Text="Confirm" OnClick="btnSubmitResetProduct_Click" />
                </div>
            </div>
        </div>
    </div>
    
    <script type="text/javascript">
        document.addEventListener('DOMContentLoaded', function () {
            const gridConfigs = [
                { id: '<%= gvListContact.ClientID %>', link: "linkDetailContact" },
                { id: '<%= gvListAddress.ClientID %>', link: "linkDetailAddress" },
                { id: '<%= gvListBusiness.ClientID %>', link: "linkDetailBusiness" },
                { id: '<%= gvListLogin.ClientID %>', link: "linkDetailLogin" },
                { id: '<%= gvListDiscount.ClientID %>', link: "linkDetailDiscount" },
                { id: '<%= gvListProduct.ClientID %>', link: "linkDetailProduct" },
                { id: '<%= gvListPromo.ClientID %>', link: "linkDetailPromo" },
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
            var tabId = selectedTab.val() != "" ? selectedTab.val() : "list-contact";
            $('#dvTab a[href="#' + tabId + '"]').tab('show');
            $("#dvTab a").click(function () {
                selectedTab.val($(this).attr("href").substring(1));
            });

            $("#listContact").on("click", function () {
                updateSessionValue("list-contact");
            });
            $("#listAddress").on("click", function () {
                updateSessionValue("list-address");
            });
            $("#listBusiness").on("click", function () {
                updateSessionValue("list-business");
            });
            $("#listLogin").on("click", function () {
                updateSessionValue("list-login");
            });
            $("#listDiscount").on("click", function () {
                updateSessionValue("list-discount");
            });
            $("#listPromo").on("click", function () {
                updateSessionValue("list-promo");
            });
            $("#listProduct").on("click", function () {
                updateSessionValue("list-product");
            });
            $("#listQuote").on("click", function () {
                updateSessionValue("list-quote");
            });
        });

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

        function showCreateOrder() {
            $("#modalCreateOrder").modal("show");
        }

        // START CUSTOMER CONTACT
        function showProcessContact() {
            $("#modalProcessContact").modal("show");
        }

        function showDeleteContact(id) {
            document.getElementById("<%=txtIdContactDelete.ClientID %>").value = id;
        }

        function showPrimaryContact(id) {
            document.getElementById("<%=txtIdPrimaryContact.ClientID %>").value = id;
        }

        // END CUSTOMER CONTACT

        // START CUSTOMER ADDRESS
        function showProcessAddress() {
            $("#modalProcessAddress").modal("show");
        }

        function showDeleteAddress(id) {
            document.getElementById("<%=txtIdAddressDelete.ClientID %>").value = id;
        }

        function showPrimaryAddress(id) {
            document.getElementById("<%=txtIdPrimaryAddress.ClientID %>").value = id;
        }
        // END CUSTOMER ADDRESS

        // START CUSTOMER BUSINESS
        function showProcessBusiness() {
            $("#modalProcessBusiness").modal("show");
        }

        function showDeleteBusiness(id) {
            document.getElementById("<%=txtIdBusinessDelete.ClientID %>").value = id;
        }

        function showPrimaryBusiness(id) {
            document.getElementById("<%=txtIdPrimaryBusiness.ClientID %>").value = id;
        }
        // END CUSTOMER BUSINESS

        // START CUSTOMER LOGIN
        function showProcessLogin() {
            $("#modalProcessLogin").modal("show");
        }

        function showInstallerAccess() {
            $("#modalInstallerAccess").modal("show");
        }

        function showActiveLogin(id, active) {
            document.getElementById("<%=txtIdActiveLogin.ClientID %>").value = id;
            document.getElementById("<%=txtActiveLogin.ClientID %>").value = active;

            let title = "";
            if (active === "1") {
                title = "Disable Customer Login";
            } else {
                title = "Enable Customer Login";
            }
            document.getElementById("titleActiveLogin").innerHTML = title;
        }     

        function showResetPass(id, username) {
            let newPass = generateNewPassword(15);
            let result = `Hi <b><%: Session("FullName") %></b>,<br />Are you sure you want to reset this account password?<br /><br /><b>USERNAME : ${username.toUpperCase()}</b><br /><b>USER ID : ${id.toUpperCase()}</b><br/><br />NEW PASSWORD : <br/><b>${newPass}</b>`;

            document.getElementById("<%=txtIdResetPass.ClientID %>").value = id;
            document.getElementById("<%=txtNewResetPass.ClientID %>").value = newPass;
            document.getElementById("spanDescResetPass").innerHTML = result;
        }

        function generateNewPassword(length) {
            const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            let result = "";
            const cryptoArray = new Uint8Array(length);
            window.crypto.getRandomValues(cryptoArray);
            for (let i = 0; i < length; i++) {
                result += chars[cryptoArray[i] % chars.length];
            }
            return result;
        }

        function showDencryptPass(username, password) {
            let body = "UserName";
            body += "<br />";
            body += "<b>" + username + "</b>";
            body += "<br /><br />";
            body += "Password Decryption";
            body += "<br />";
            body += "<b>" + password + "</b>";
            document.getElementById("spanPassword").innerHTML = body;
        }
        // END CUSTOMER LOGIN

        // START CUSTOMER DISCOUNT
        function showProcessDiscount() {
            $("#modalProcessDiscount").modal("show");
        }

        function hideAllDiscountInputs() {
            var divType = document.getElementById("divDiscountType");
            var divA = document.getElementById("divDiscountDataId");
            var divB = document.getElementById("divDiscountDataIdB");

            if (divType) divType.style.display = "none";
            if (divA) divA.style.display = "none";
            if (divB) divB.style.display = "none";
        }

        function visibleDiscountType() {
            var type = document.getElementById("ddlDiscountType");
            var divType = document.getElementById("divDiscountType");
            var divA = document.getElementById("divDiscountDataId");
            var divB = document.getElementById("divDiscountDataIdB");

            if (!type) return;
            if (divType) divType.style.display = "block";

            if (type.value === "Designs") {
                if (divA) divA.style.display = "block";
                if (divB) divB.style.display = "none";
            } else if (type.value === "PriceProductGroups") {
                if (divA) divA.style.display = "none";
                if (divB) divB.style.display = "block";
            } else {
                if (divA) divA.style.display = "none";
                if (divB) divB.style.display = "none";
            }
        }

        function showDeleteDiscount(id) {
            document.getElementById("<%=txtIdDiscountDelete.ClientID %>").value = id;
        }
        // END CUSTOMER DISCOUNT

        // START CUSTOMER PROMO
        function showProcessPromo() {
            $("#modalProcessPromo").modal("show");
        }

        function showDetailPromo() {
            $("#modalDetailPromo").modal("show");
        }

        function showDeletePromo(id) {
            document.getElementById("<%=txtIdPromoDelete.ClientID %>").value = id;
        }
        // END CUSTOMER PROMO


        // CUSTOMER PRODUCT ACCESS
        function showProcessProduct() {
            $("#modalProcessProduct").modal("show");
        }

        [
            "modalDelete", "modalCreateOrder", "modalLog",
            "modalProcessContact", "modalDeleteContact", "modalPrimaryContact",
            "modalProcessAddress", "modalDeleteAddress", "modalPrimaryAddress",
            "modalProcessBusiness", "modalDeleteBusiness", "modalPrimaryBusiness",
            "modalProcessLogin", "modalInstallerAccess", "modalActiveLogin", "modalResetPass", "modalDencryptPass", "modalCredentialsLogin",
            "modalProcessDiscount", "modalResetDiscount", "modalDeleteDiscount",
            "modalProcessPromo", "modalDetailPromo", "modalResetPromo", "modalDeletePromo",
            "modalResetProduct", "modalProcessProduct"
        ].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });

        window.history.replaceState(null, null, window.location.href);
    </script>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblIdContact"></asp:Label>
        <asp:Label runat="server" ID="lblActionContact"></asp:Label>

        <asp:Label runat="server" ID="lblIdAddress"></asp:Label>
        <asp:Label runat="server" ID="lblActionAddress"></asp:Label>

        <asp:Label runat="server" ID="lblIdBusiness"></asp:Label>
        <asp:Label runat="server" ID="lblActionBusiness"></asp:Label>

        <asp:Label runat="server" ID="lblIdLogin"></asp:Label>        
        <asp:Label runat="server" ID="lblActionLogin"></asp:Label>
        <asp:Label runat="server" ID="lblLoginUserNameOld"></asp:Label>

        <asp:Label runat="server" ID="lblIdDiscount"></asp:Label>
        <asp:Label runat="server" ID="lblActionDiscount" ClientIDMode="Static"></asp:Label>

        <asp:Label runat="server" ID="lblIdPromo"></asp:Label>
        <asp:Label runat="server" ID="lblActionPromo"></asp:Label>

        <asp:Label runat="server" ID="lblIdProduct"></asp:Label>
    </div>
</asp:Content>