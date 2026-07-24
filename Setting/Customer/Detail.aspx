<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Detail.aspx.vb" Inherits="Setting_Customer_Detail" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Customer Detail" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .text-muted { font-size: 0.875rem; }
        .fw-semibold { font-weight: 600; }
        h6.text-uppercase { letter-spacing: .5px; border-bottom: 1px solid #dee2e6; padding-bottom: .5rem; }
    </style>

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
                            <li class="breadcrumb-item"><a runat="server" href="~/setting/customer/list">List</a></li>
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
                <a href="javascript:void(0);" runat="server" id="aDelete" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalDelete">Delete</a>
                <a href="javascript:void(0);" runat="server" id="aWelcome" class="btn btn-success" data-bs-toggle="modal" data-bs-target="#modalWelcome">Welcome</a>
                <a href="javascript:void(0);" class="btn btn-info" data-bs-toggle="modal" data-bs-target="#modalSendLogin">Send Login</a>
                <a href="javascript:void(0);" class="btn btn-dark" data-bs-toggle="modal" data-bs-target="#modalRecalculate">Re-Price Order</a>
                <a href="javascript:void(0);" class="btn btn-secondary" onclick="showLog('Customers', '<%= lblId.Text %>')">Log</a>
            </div>
        </section>
        <section class="row" runat="server" id="secDetail">
            <div class="col-12">
                <div class="card">
                    <div class="card-content">
                        <div class="card-body">
                            <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                <a class="list-group-item list-group-item-action active" id="listGeneral" data-bs-toggle="list" href="#list-general" role="tab">General</a>
                                <a class="list-group-item list-group-item-action" id="listContact" data-bs-toggle="list" href="#list-contact" role="tab">Contact</a>
                                <a class="list-group-item list-group-item-action" id="listAddress" data-bs-toggle="list" href="#list-address" role="tab">Address</a>
                                <a class="list-group-item list-group-item-action" id="listBusiness" data-bs-toggle="list" href="#list-business" role="tab">Business</a>
                                <a class="list-group-item list-group-item-action" id="listLogin" data-bs-toggle="list" href="#list-login" role="tab">Login</a>
                                <a class="list-group-item list-group-item-action" id="listDiscount" data-bs-toggle="list" href="#list-discount" role="tab">Discount</a>
                                <a class="list-group-item list-group-item-action" id="listPromo" data-bs-toggle="list" href="#list-promo" role="tab">Promo</a>
                                <a class="list-group-item list-group-item-action" id="listProduct" data-bs-toggle="list" href="#list-product" role="tab">Product</a>
                                <a class="list-group-item list-group-item-action" id="listQuote" data-bs-toggle="list" href="#list-quote" role="tab">Quote</a>
                            </div>
                            <div class="tab-content text-justify">
                                <div class="tab-pane fade show active" id="list-general" role="tabpanel" aria-labelledby="listGeneral">
                                    <div class="mt-4 border-bottom pb-3 mb-4">
                                        <h3 class="fw-bold mb-1">
                                            <asp:Label runat="server" ID="lblName"></asp:Label>
                                        </h3>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <h6 class="text-uppercase fw-bold text-primary mb-3">Company Information</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Main Company</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblCompanyName"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Sub Company</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblCompanyDetailName"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Area</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblArea"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-4">
                                                <div class="col-4 text-muted">Sales</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblOperator"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Level</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblLevel"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-4">
                                                <div class="col-4 text-muted">Primary Account</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblPrimary"></asp:Label>
                                                </div>
                                            </div>
                                            <h6 class="text-uppercase fw-bold text-primary mb-3">Pricing Information</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Price Group</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblPriceGroup"></asp:Label>
                                                    <asp:Label runat="server" ID="lblPriceCustom" Font-Bold="true"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Shutter Price Group</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblPriceGroupShutter"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-4">
                                                <div class="col-4 text-muted">Door Price Group</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblPriceGroupDoor"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">
                                            <h6 class="text-uppercase fw-bold text-success mb-3">Status & Settings</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">On Stop</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblOnStop"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Cash Sale</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblCashSale"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Newsletter</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblNewsletter"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-4">
                                                <div class="col-4 text-muted">Minimum Surcharge</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblMinSurcharge"></asp:Label>
                                                </div>
                                            </div>
                                            <h6 class="text-uppercase fw-bold text-secondary mb-3">System Information</h6>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">ID</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblId"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Debtor Code</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblDebtorCode"></asp:Label>
                                                </div>
                                            </div>
                                            <div class="row mb-2">
                                                <div class="col-4 text-muted">Active</div>
                                                <div class="col-8 fw-semibold">
                                                    <asp:Label runat="server" ID="lblActive"></asp:Label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane fade" id="list-contact" role="tabpanel" aria-labelledby="listContact">
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
                                                <asp:GridView runat="server" ID="gvListContact" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Name" HeaderText="Name" />
                                                        <asp:BoundField DataField="Email" HeaderText="Email" />
                                                        <asp:BoundField DataField="Phone" HeaderText="Phone" />
                                                        <asp:BoundField DataField="Tags" HeaderText="Tags" />
                                                        <asp:BoundField DataField="PrimaryData" HeaderText="Primary" />
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="180px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <a class="dropdown-item" id="aDetailContact" href='<%# Page.ResolveUrl("~/setting/customer/contact/edit?contactid=" & Eval("Id") & "&returnpage=detail") %>'>Detail / Edit</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteContact" onclick='<%# String.Format("return dataDeleteContact(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# VisiblePrimaryContact(Eval("Primary")) %>'>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPrimaryContact" onclick='<%# String.Format("return dataPrimaryContact(`{0}`);", Eval("Id").ToString()) %>'>Set As Primary</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('CustomerContacts', '<%# Eval("Id") %>')">Log</a>
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
                                                <asp:GridView runat="server" ID="gvListAddress" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="ADDRESS NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Description" HeaderText="Description" />
                                                        <asp:TemplateField HeaderText="Address">
                                                            <ItemTemplate>
                                                                <%# BindDetailAddress(Eval("Id").ToString()) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="Note" HeaderText="Note" />
                                                        <asp:BoundField DataField="PrimaryData" HeaderText="Primary" />
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <a class="dropdown-item" id="aDetailAddress" href='<%# Page.ResolveUrl("~/setting/customer/address/edit?addressid=" & Eval("Id") & "&returnpage=detail") %>'>Detail / Edit</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteAddress" onclick='<%# String.Format("return dataDeleteAddress(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# VisiblePrimaryAddress(Eval("Primary")) %>'>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPrimaryAddress" onclick='<%# String.Format("return dataPrimaryAddress(`{0}`);", Eval("Id").ToString()) %>'>Set As Primary</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('CustomerAddress', '<%# Eval("Id") %>')">Log</a>
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
                                    </div>
                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <div class="table-responsive">
                                                <asp:GridView runat="server" ID="gvListBusiness" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="ABNNumber" HeaderText="ABN Number" />
                                                        <asp:BoundField DataField="RegisteredName" HeaderText="Registered Name" />
                                                        <asp:BoundField DataField="PrimaryData" HeaderText="Primary" />
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <a class="dropdown-item" id="aDetailBusiness" href='<%# Page.ResolveUrl("~/setting/customer/business/edit?businessid=" & Eval("Id") & "&returnpage=detail") %>'>Detail / Edit</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteBusiness" onclick='<%# String.Format("return dataDeleteBusiness(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# VisiblePrimaryBusiness(Eval("Primary")) %>'>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalPrimaryBusiness" onclick='<%# String.Format("return dataPrimaryBusiness(`{0}`);", Eval("Id").ToString()) %>'>Set As Primary</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('CustomerBusiness', '<%# Eval("Id") %>')">Log</a>
                                                                    </li>
                                                                </ul>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                    </Columns>
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mt-5">
                                        <div class="col-12">
                                            <asp:Button runat="server" ID="btnAddBusiness" CssClass="btn btn-primary" Text="Add New" OnClick="btnAddBusiness_Click" />
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
                                                <asp:GridView runat="server" ID="gvListLogin" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="LOGIN NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Role">
                                                            <ItemTemplate>
                                                                <%# If(IsDBNull(Eval("RoleName")) OrElse IsDBNull(Eval("LevelName")) OrElse String.IsNullOrEmpty(Eval("RoleName") & "") OrElse String.IsNullOrEmpty(Eval("LevelName") & ""), "Requires correction", Eval("RoleName") & " - " & Eval("LevelName")) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="UserName" HeaderText="User" />
                                                        <asp:BoundField DataField="FullName" HeaderText="Full Name" />
                                                        <asp:BoundField DataField="LastLogin" HeaderText="Last Login" DataFormatString="{0:dd MMM yyyy HH:mm:ss}" />
                                                        <asp:BoundField DataField="DataPricing" HeaderText="Pricing" />
                                                        <asp:TemplateField HeaderText="Status">
                                                            <ItemTemplate>
                                                                <asp:Label ID="lblStatus" runat="server"
                                                                    Text='<%# Eval("DataActive") %>'
                                                                    ForeColor='<%# If(Eval("DataActive").ToString() = "Disabled", Drawing.Color.Red, Nothing) %>'>
                                                                </asp:Label>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <a class="dropdown-item" id="aDetailLogin" href='<%# Page.ResolveUrl("~/setting/customer/login/edit?loginid=" & Eval("Id") & "&returnpage=detail") %>'>Detail / Edit</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalActiveLogin" onclick='<%# String.Format("return dataActiveLogin(`{0}`, `{1}`);", Eval("Id").ToString(), Convert.ToInt32(Eval("Active"))) %>'><%# TextActive_Login(Eval("Active")) %></a>
                                                                    </li>
                                                                    <li runat="server" visible='<%# VisibleSendPersonalLogin(Convert.ToInt32(Eval("Active"))) %>'>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalSendPersonalLogin" onclick='<%# String.Format("return dataSendPersonalLogin(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("Email").ToString()) %>'>Send Personal Login</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalChangePasswordLogin" onclick='<%# String.Format("return dataChangePasswordLogin(`{0}`);", Eval("Id").ToString()) %>'>Change Password</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalResetPasswordLogin" onclick='<%# String.Format("return dataResetPasswordLogin(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("UserName").ToString()) %>'>Reset Password</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('Logins', '<%# Eval("Id") %>')">Log</a>
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
                                                <asp:GridView runat="server" ID="gvListDiscount" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
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
                                                                    <li>
                                                                        <a class="dropdown-item" id="aDetailDiscount" href='<%# Page.ResolveUrl("~/setting/customer/discount/edit?discountid=" & Eval("Id") & "&returnpage=detail") %>'>Detail / Edit</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeleteDiscount" onclick='<%# String.Format("return dataDeleteDiscount(`{0}`);", Eval("Id").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('CustomerDiscounts', '<%# Eval("Id") %>')">Log</a>
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
                                            <button class="btn btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false" runat="server" id="btnAddDiscount">Add Discount</button>
                                            <ul class="dropdown-menu">
                                                <li>
                                                    <asp:Button runat="server" ID="btnAddDiscountA" CssClass="dropdown-item" Text="By Product" OnClick="btnAddDiscountA_Click" />
                                                </li>
                                                <li>
                                                    <asp:Button runat="server" ID="btnAddDiscountB" CssClass="dropdown-item" Text="By Product Group" OnClick="btnAddDiscountB_Click" />
                                                </li>
                                            </ul>
                                            <a href="javascript:void(0);" runat="server" id="aResetDiscount" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#modalResetDiscount">Reset Discount</a>
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
                                                <asp:GridView runat="server" ID="gvListPromo" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:BoundField DataField="PromoName" HeaderText="Promo" />
                                                         <asp:BoundField DataField="StartDate" HeaderText="Start Date" DataFormatString="{0:dd MMM yyyy}" />
                                                        <asp:BoundField DataField="EndDate" HeaderText="End Date" DataFormatString="{0:dd MMM yyyy}" />
                                                        <asp:TemplateField ItemStyle-Width="120px">
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <a href="javascript:void(0);" id="aDetailPromo" class="dropdown-item" onclick="showDetailPromo('<%# Eval("Id") %>');">Detail</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalDeletePromo" onclick='<%# String.Format("return dataDeletePromo(`{0}`, `{1}`);", Eval("Id").ToString(), Eval("PromoId").ToString()) %>'>Delete</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('CustomerPromos', '<%# Eval("Id") %>')">Log</a>
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
                                                <asp:GridView runat="server" ID="gvListProduct" CssClass="table table-bordered table-hover mb-0" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center">
                                                    <Columns>
                                                        <asp:TemplateField ItemStyle-HorizontalAlign="Center" ItemStyle-Width="60px">
                                                            <ItemTemplate>
                                                                <%# Container.DataItemIndex + 1 %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField HeaderText="Product" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemTemplate>
                                                                <%# BindDetailProduct(Eval("Id").ToString()) %>
                                                            </ItemTemplate>
                                                        </asp:TemplateField>
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <button class="btn btn-sm btn-primary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">Action</button>
                                                                <ul class="dropdown-menu">
                                                                    <li>
                                                                        <a class="dropdown-item" id="aDetailProduct" href='<%# Page.ResolveUrl("~/setting/customer/product/edit?productid=" & Eval("Id") & "&returnpage=detail") %>'>Detail / Edit</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" runat="server" class="dropdown-item" data-bs-toggle="modal" data-bs-target="#modalResetProduct">Reset</a>
                                                                    </li>
                                                                    <li>
                                                                        <a href="javascript:void(0);" class="dropdown-item" onclick="showLog('CustomerProductAccess', '<%# Eval("Id") %>')">Log</a>
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
                                                        <asp:TemplateField>
                                                            <ItemTemplate>
                                                                <a href="javascript:void(0);" class="btn btn-sm btn-primary" onclick="showLog('CustomerQuotes', '<%# Eval("Id") %>')">Log</a>
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

    <asp:HiddenField ID="selected_tab" runat="server" />
    
    <div class="modal modal-blur fade" id="modalRecalculate" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-dark">
                    <h5 class="modal-title white">Re-Price Order</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />
                    Teks
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnRecalculate" CssClass="btn btn-dark" OnClick="btnRecalculate_Click" Text="Confirm" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalWelcome" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-success">
                    <h5 class="modal-title white">Send Welcome Email</h5>
                </div>
                <div class="modal-body text-center py-4">
                    Hi <b><%: Session("FullName") %></b>,<br />All login details will be sent to the primary contact email address.<br /><br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnWelcome" CssClass="btn btn-success" Text="Confirm" OnClick="btnWelcome_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalSendLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Send Login</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Email Address</label>
                            <asp:TextBox runat="server" ID="txtSendLoginEmail" CssClass="form-control" placeholder="Email Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorSendLogin">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorSendLogin"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSendLogin" Text="Submit" CssClass="btn btn-info" OnClick="btnSendLogin_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
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
    <div class="modal modal-blur fade" id="modalDeleteContact" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white">Delete Customer Contact</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtDeleteContactId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <asp:TextBox runat="server" ID="txtPrimaryContactId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnPrimaryContact" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnPrimaryContact_Click" />
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
                    <asp:TextBox runat="server" ID="txtAddressDeleteId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <asp:TextBox runat="server" ID="txtPrimaryAddressId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnPrimaryAddress" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnPrimaryAddress_Click" />
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
                    <asp:TextBox runat="server" ID="txtBusinessDeleteId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <asp:TextBox runat="server" ID="txtPrimaryBusinessId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnPrimaryBusiness" CssClass="btn btn-secondary" Text="Confirm" OnClick="btnPrimaryBusiness_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalActiveLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-danger">
                    <h5 class="modal-title white" id="titleActiveLogin"></h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtActiveLoginId" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtActiveLoginStatus" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnActiveLogin" CssClass="btn btn-danger" Text="Confirm" OnClick="btnActiveLogin_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalSendPersonalLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Send Personal Login</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtSendPersonalLoginId" style="display:none;"></asp:TextBox>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">Email Address</label>
                            <asp:TextBox runat="server" ID="txtSendPersonalLoginEmail" CssClass="form-control" placeholder="Email Address ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                    <div class="row" runat="server" id="divErrorSendPersonalLogin">
                        <div class="col-12">
                            <div class="alert alert-danger">
                                <span runat="server" id="msgErrorSendPersonalLogin"></span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSendPersonalLogin" Text="Submit" CssClass="btn btn-primary" OnClick="btnSendPersonalLogin_Click" OnClientClick="return showWaiting();" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalChangePasswordLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-dialog-centered modal-dialog-scrollable" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Change Password</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:TextBox runat="server" ID="txtChangePasswordLoginId" style="display:none;"></asp:TextBox>
                    <div class="row">
                        <div class="col-12 form-group">
                            <label class="form-label">New Password</label>
                            <asp:TextBox runat="server" ID="txtChangePassword" CssClass="form-control" placeholder="Password ..." autocomplete="off"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnChangePasswordLogin" Text="Submit" CssClass="btn btn-primary" OnClick="btnChangePasswordLogin_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalResetPasswordLogin" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-sm modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header bg-info">
                    <h5 class="modal-title white">Reset Password</h5>
                </div>
                <div class="modal-body text-center py-4">
                    <asp:TextBox runat="server" ID="txtResetPasswordLoginId" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtResetPasswordLoginNew" style="display:none;"></asp:TextBox>
                    <span id="spanResetPasswordLoginDesc"></span>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnResetPasswordLogin" CssClass="btn btn-info" Text="Confirm" OnClick="btnResetPasswordLogin_Click" />
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
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
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
                    <asp:TextBox runat="server" ID="txtDeleteDiscountId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeleteDiscount" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeleteDiscount_Click" />
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal-blur fade" id="modalDetailPromo" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog modal-lg modal-dialog-centered" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Detail Promo</h5>
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-12">
                            <div class="table-responsive">
                                <table class="table table-bordered table-hover mb-0">
                                    <thead>
                                        <tr>
                                            <th style="width:60px;"></th>
                                            <th>Type</th>
                                            <th>Discount</th>
                                        </tr>
                                    </thead>
                                    <tbody id="promoDetailBody"></tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Close</a>
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
                    <asp:TextBox runat="server" ID="txtDeletePromoId" style="display:none;"></asp:TextBox>
                    <asp:TextBox runat="server" ID="txtDeleteDetailPromoId" style="display:none;"></asp:TextBox>
                    Hi <b><%: Session("FullName") %></b>,<br />Are you sure you would like to do this?
                </div>
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnDeletePromo" CssClass="btn btn-danger" Text="Confirm" OnClick="btnDeletePromo_Click" />
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
                    <a href="javascript:void(0);" class="btn btn-light-secondary" data-bs-dismiss="modal">Cancel</a>
                    <asp:Button runat="server" ID="btnSubmitResetProduct" CssClass="btn btn-danger" Text="Confirm" OnClick="btnSubmitResetProduct_Click" />
                </div>
            </div>
        </div>
    </div>

    <div runat="server" visible="false">
        <asp:Label runat="server" ID="lblIdContact"></asp:Label>
        <asp:Label runat="server" ID="lblIdAddress"></asp:Label>
        <asp:Label runat="server" ID="lblIdBusiness"></asp:Label>
        <asp:Label runat="server" ID="lblIdLogin"></asp:Label>
        <asp:Label runat="server" ID="lblIdDiscount"></asp:Label>
        <asp:Label runat="server" ID="lblIdPromo"></asp:Label>

        <asp:Label runat="server" ID="lblCompanyId"></asp:Label>
        <asp:Label runat="server" ID="lblCompanyDetailId"></asp:Label>
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
        document.addEventListener('DOMContentLoaded', function () {
            const gridConfigs = [
                { id: '<%= gvListContact.ClientID %>', link: "aDetailContact" },
                { id: '<%= gvListAddress.ClientID %>', link: "aDetailAddress" },
                { id: '<%= gvListBusiness.ClientID %>', link: "aDetailBusiness" },
                { id: '<%= gvListLogin.ClientID %>', link: "aDetailLogin" },
                { id: '<%= gvListDiscount.ClientID %>', link: "aDetailDiscount" },
                { id: '<%= gvListProduct.ClientID %>', link: "aDetailProduct" },
                { id: '<%= gvListPromo.ClientID %>', link: "aDetailPromo" },
            ];
            gridConfigs.forEach(cfg => {
                const gv = document.getElementById(cfg.id);
                if (!gv) return;
                for (let i = 1; i < gv.rows.length; i++) {
                    const row = gv.rows[i];
                    row.style.cursor = 'pointer';
                    row.addEventListener('click', function (e) {
                        if (e.target.closest("a") || e.target.closest("button") || e.target.closest("[data-bs-toggle]")) {
                            return;
                        }
                        const btn = this.querySelector(`a[id*='${cfg.link}']`);
                        if (btn) btn.click();
                    });
                }
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
        function showSendLogin() {
            $("#modalSendLogin").modal("show");
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
        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 3000);

            return true;
        }
        function dataDeleteContact(contactid) {
            document.getElementById("<%=txtDeleteContactId.ClientID %>").value = contactid;
        }
        function dataPrimaryContact(contactid) {
            document.getElementById("<%=txtPrimaryContactId.ClientID %>").value = contactid;
        }
        function dataDeleteAddress(addressid) {
            document.getElementById("<%=txtAddressDeleteId.ClientID %>").value = addressid;
        }
        function dataPrimaryAddress(addressid) {
            document.getElementById("<%=txtPrimaryAddressId.ClientID %>").value = addressid;
        }
        function dataDeleteBusiness(businessid) {
            document.getElementById("<%=txtBusinessDeleteId.ClientID %>").value = businessid;
        }
        function dataPrimaryBusiness(businessid) {
            document.getElementById("<%=txtPrimaryBusinessId.ClientID %>").value = businessid;
        }
        function dataActiveLogin(loginid, status) {
            document.getElementById("<%=txtActiveLoginId.ClientID %>").value = loginid;
            document.getElementById("<%=txtActiveLoginStatus.ClientID %>").value = status;

            let title = "";
            if (status === "1") {
                title = "Disable Customer Login";
            } else {
                title = "Enable Customer Login";
            }
            document.getElementById("titleActiveLogin").innerHTML = title;
        }
        function dataSendPersonalLogin(loginid, email) {
            document.getElementById("<%=txtSendPersonalLoginId.ClientID %>").value = loginid;
            document.getElementById("<%=txtSendPersonalLoginEmail.ClientID %>").value = email;
        }
        function showSendPersonalLogin() {
            $("#modalSendPersonalLogin").modal("show");
        }
        function dataChangePasswordLogin(loginid) {
            document.getElementById("<%=txtChangePasswordLoginId.ClientID %>").value = loginid;
        }
        function dataResetPasswordLogin(loginid, username) {
            let newPass = generateNewPassword(15);
            let result = `Hi <b><%: Session("FullName") %></b>,<br />Are you sure you want to reset this account password?<br /><br /><b>USERNAME : ${username.toUpperCase()}</b><br /><b>USER ID : ${loginid.toUpperCase()}</b><br/><br />NEW PASSWORD : <br/><b>${newPass}</b>`;

            document.getElementById("<%=txtResetPasswordLoginId.ClientID %>").value = loginid;
            document.getElementById("<%=txtResetPasswordLoginNew.ClientID %>").value = newPass;
            document.getElementById("spanResetPasswordLoginDesc").innerHTML = result;
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
        function dataDeleteDiscount(discountid) {
            document.getElementById("<%=txtDeleteDiscountId.ClientID %>").value = discountid;
        }
        function showDetailPromo(id) {
            $.ajax({
                type: "POST",
                url: "Detail.aspx/GetPromoDetail",
                data: JSON.stringify({id: id}),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (res) {
                    let html = "";
                    if (res.d.length > 0) {
                        $.each(res.d, function (i, item) {
                            html += `
                            <tr>
                            <td class="text-center">${i + 1}</td>
                            <td>${item.Type}</td>
                            <td>${item.Discount}</td>
                            </tr>`;
                        });
                    } else {
                        html = `
                        <tr>
                        <td colspan="3" class="text-center">DATA NOT FOUND :)</td>
                        </tr>`;
                    }
                    $("#promoDetailBody").html(html);
                    $("#modalDetailPromo").modal("show");
                },
                error: function (xhr) {
                    console.log(xhr);
                }
            });
        }
        function dataDeletePromo(promoid, detailid) {
            document.getElementById("<%=txtDeletePromoId.ClientID %>").value = promoid;
            document.getElementById("<%=txtDeleteDetailPromoId.ClientID %>").value = detailid;
        }
        [
            "modalDelete", "modalRecalculate", "modalLog", "modalWelcome", "modalSendLogin", "modalWaiting",
            "modalDeleteContact", "modalPrimaryContact",
            "modalDeleteAddress", "modalPrimaryAddress",
            "modalDeleteBusiness", "modalPrimaryBusiness",
            "modalActiveLogin", "modalSendPersonalLogin", "modalChangePasswordLogin", "modalResetPasswordLogin",
            "modalResetDiscount", "modalDeleteDiscount",
            "modalDetailPromo", "modalDeletePromo",
            "modalResetProduct"
        ].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>