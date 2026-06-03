<%@ Page Language="VB" AutoEventWireup="false" CodeFile="JPMDStock.aspx.vb" Inherits="JPMDStock" Debug="true" %>

<html lang="en">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Stock</title>

    <link rel="stylesheet" href="/Assets/vendors/choices.js/choices.min.css" />
    <link rel="stylesheet" href="/Assets/css/bootstrap.css" />
    <link rel="stylesheet" href="/Assets/css/widgets/chat.css" />
    <link rel="stylesheet" href="/Assets/vendors/sweetalert2/sweetalert2.min.css" />
    <link rel="stylesheet" href="/Assets/vendors/iconly/bold.css" />
    <link rel="stylesheet" href="/Assets/vendors/perfect-scrollbar/perfect-scrollbar.css" />
    <link rel="stylesheet" href="/Assets/vendors/bootstrap-icons/bootstrap-icons.css" />
    <link rel="stylesheet" href="/Assets/css/app.css" />

    <style>
        .grid-container { width: 100%; height: calc(100vh - 150px); overflow: auto; border: 1px solid #ddd; }
        .grid-container table { width: 100%; border-collapse: collapse; table-layout: auto; }
        .grid-container td, .grid-container th { white-space: nowrap; padding: 6px 10px; }
        .grid-container th { position: sticky; top: 0; background: #f5f5f5; z-index: 3; }
        .grid-container td:first-child, .grid-container th:first-child { position: sticky; left: 0; background: #fff; z-index: 2; white-space: normal; word-break: break-word; min-width: 220px; max-width: 320px; }
        .grid-container th:first-child { z-index: 5; background: #f5f5f5; }
        .grid-container tr:last-child td { position: sticky; bottom: 0; background: #f5f5f5; z-index: 3; }
        .grid-container tr:last-child td:first-child { left: 0; z-index: 6; background: #f5f5f5; }
    </style>
    
    <link rel="icon" type="image/x-icon" href="/Assets/images/logo/boos.ico" />

    <script src="/Scripts/jquery-3.7.1.min.js"></script>
</head>
<body>
    <form runat="server">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <div id="app">
            <div id="main" class="layout-horizontal">
                <header class="mb-5">
                    <div class="header-top">
                        <div class="container">
                            <div class="logo">
                                <a runat="server" href="~/">
                                    <asp:Image runat="server" ID="imgLogo" ImageUrl="~/Assets/images/logo/jpmdirect.jpg?v=1.0.0" AlternateText="JPM Direct Pty Ltd" />
                                </a>
                            </div>
                            <div class="header-top-right">
                                <div class="dropdown">
                                    <a href="#" class="user-dropdown d-flex dropend" data-bs-toggle="dropdown" aria-expanded="false">
                                        <div class="avatar avatar-md2">
                                            <img runat="server" src="~/Assets/images/avatars.png" alt="Avatar">
                                        </div>
                                        <div class="text">
                                            <h6 class="user-dropdown-name">Guest</h6>
                                            <p class="user-dropdown-status text-sm text-muted">Guest</p>
                                        </div>
                                    </a>
                                    <ul class="dropdown-menu dropdown-menu-end shadow-lg">
                                        <li><a class="dropdown-item" runat="server" href="~/">Login</a></li>
                                    </ul>
                                </div>
                                <a href="#" class="burger-btn d-block d-xl-none">
                                    <i class="bi bi-justify fs-3"></i>
                                </a>
                            </div>
                        </div>
                    </div>
                    <nav class="main-navbar">
                        <div class="container">
                            <ul>
                                <li class="menu-item">
                                    <a runat="server" href="~/" class='menu-link'>
                                        <i class="bi bi-house-door-fill"></i>
                                        <span>Home</span>
                                    </a>
                                </li>
                                <li class="menu-item">
                                    <a runat="server" href="~/jpmdstock" class='menu-link'>
                                        <i class="bi bi-ui-checks"></i>
                                        <span>Stocks</span>
                                    </a>
                                </li>
                            </ul>
                        </div>
                    </nav>
                </header>
                <div class="content-wrapper container">
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
                        <section class="row">
                            <div class="col-12">
                                <div class="card">
                                    <div class="card-content">
                                        <div class="card-body">
                                            <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                                <a class="list-group-item list-group-item-action active" id="listRoller" data-bs-toggle="list" href="#list-roller" role="tab">Roller</a>
                                                <a class="list-group-item list-group-item-action" id="listVertical" data-bs-toggle="list" href="#list-vertical" role="tab">Vertical</a>
                                                <a class="list-group-item list-group-item-action" id="listCellular" data-bs-toggle="list" href="#list-cellular" role="tab">Cellular Shades</a>
                                                <a class="list-group-item list-group-item-action" id="listProfile" data-bs-toggle="list" href="#list-profile" role="tab">Design Shades</a>
                                                <a class="list-group-item list-group-item-action" id="listCurtain" data-bs-toggle="list" href="#list-curtain" role="tab">Curtain</a>
                                                <a class="list-group-item list-group-item-action" id="listVenetian" data-bs-toggle="list" href="#list-venetian" role="tab">Venetian Blind</a>
                                                <a class="list-group-item list-group-item-action" id="listAluminium" data-bs-toggle="list" href="#list-aluminium" role="tab">Aluminium Blind</a>
                                                <a class="list-group-item list-group-item-action" id="listFabricChart" data-bs-toggle="list" href="#list-fabricchart" role="tab">Fabric Chart</a>
                                            </div>
                                            <div class="tab-content text-justify">
                                                <div class="tab-pane fade show active" id="list-roller" role="tabpanel" aria-labelledby="listRoller">
                                                    <div class="row mt-5" runat="server" id="divErrorRoller">
                                                        <div class="col-12">
                                                            <div class="alert alert-danger">
                                                                <span runat="server" id="msgErrorRoller"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5" runat="server">
                                                        <div class="col-12 col-sm-12 col-lg-7">
                                                            <asp:Panel runat="server" DefaultButton="btnSearchRoller" Width="100%">
                                                                <div class="input-group">
                                                                    <span class="input-group-text">Fabric Type : </span>
                                                                    <asp:TextBox runat="server" ID="txtSearchRoller" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                                    <asp:Button runat="server" ID="btnSearchRoller" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchRoller_Click" />
                                                                </div>
                                                            </asp:Panel>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <table class="table mb-0">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                            <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                            <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                            <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive grid-container">
                                                                <asp:GridView runat="server" ID="gvListRoller" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListRoller_RowDataBound">
                                                                    <RowStyle />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="list-profile" role="tabpanel" aria-labelledby="listProfile">
                                                    <div class="row mt-5" runat="server" id="divErrorProfile">
                                                        <div class="col-12">
                                                            <div class="alert alert-danger">
                                                                <span runat="server" id="msgErrorProfile"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <table class="table mb-0">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                            <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                            <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                            <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive grid-container">
                                                                <asp:GridView runat="server" ID="gvListProfile" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListProfile_RowDataBound">
                                                                    <RowStyle />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="list-curtain" role="tabpanel" aria-labelledby="listCurtain">
                                                    <div class="row mt-5" runat="server" id="divErrorCurtain">
                                                        <div class="col-12">
                                                            <div class="alert alert-danger">
                                                                <span runat="server" id="msgErrorCurtain"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12 col-sm-12 col-lg-7">
                                                            <asp:Panel runat="server" DefaultButton="btnSearchCurtain" Width="100%">
                                                                <div class="input-group">
                                                                    <span class="input-group-text">Fabric Type : </span>
                                                                    <asp:TextBox runat="server" ID="txtSearchCurtain" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                                    <asp:Button runat="server" ID="btnSearchCurtain" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchCurtain_Click" />
                                                                </div>
                                                            </asp:Panel>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <table class="table mb-0">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                            <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                            <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                            <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive grid-container">
                                                                <asp:GridView runat="server" ID="gvListCurtain" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListCurtain_RowDataBound">
                                                                    <RowStyle />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="list-vertical" role="tabpanel" aria-labelledby="listVertical">
                                                    <div class="row mt-5" runat="server" id="divErrorVertical">
                                                        <div class="col-12">
                                                            <div class="alert alert-danger">
                                                                <span runat="server" id="msgErrorVertical"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12 col-sm-12 col-lg-7">
                                                            <asp:Panel runat="server" DefaultButton="btnSearchVertical" Width="100%">
                                                                <div class="input-group">
                                                                    <span class="input-group-text">Fabric Type : </span>
                                                                    <asp:TextBox runat="server" ID="txtSearchVertical" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                                    <asp:Button runat="server" ID="btnSearchVertical" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchVertical_Click" />
                                                                </div>
                                                            </asp:Panel>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <table class="table mb-0">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                            <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                            <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                            <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive grid-container">
                                                                <asp:GridView runat="server" ID="gvListVertical" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListVertical_RowDataBound">
                                                                    <RowStyle />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="list-venetian" role="tabpanel" aria-labelledby="listVenetian">
                                                    <div class="row mt-5" runat="server" id="divErrorVenetian">
                                                        <div class="col-12">
                                                            <div class="alert alert-danger">
                                                                <span runat="server" id="msgErrorVenetian"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <table class="table mb-0">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                            <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                            <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                            <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive grid-container">
                                                                <asp:GridView runat="server" ID="gvListVenetian" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListVenetian_RowDataBound">
                                                                    <RowStyle />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="BlindName" HeaderText="" />
                                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="list-aluminium" role="tabpanel" aria-labelledby="listAluminium">
                                                    <div class="row mt-5" runat="server" id="divErrorAluminium">
                                                        <div class="col-12">
                                                            <div class="alert alert-danger">
                                                                <span runat="server" id="msgErrorAluminium"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <table class="table mb-0">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                            <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                            <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                            <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive grid-container">
                                                                <asp:GridView runat="server" ID="gvListAluminium" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListAluminium_RowDataBound">
                                                                    <RowStyle />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="BlindName" HeaderText="" />
                                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="list-cellular" role="tabpanel" aria-labelledby="listCellular">
                                                    <div class="row mt-5" runat="server" id="divErrorCellular">
                                                        <div class="col-12">
                                                            <div class="alert alert-danger">
                                                                <span runat="server" id="msgErrorCellular"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-5">
                                                        <div class="col-12">
                                                            <div class="table-responsive">
                                                                <table class="table mb-0">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td style="background-color: darkgreen; color: white; text-align:center;">IN STOCK</td>
                                                                            <td style="background-color: yellow; color:black; text-align:center;">LIMITED STOCK</td>
                                                                            <td style="background-color: darkred; color: white; text-align:center;">OUT OF STOCK</td>
                                                                            <td style="background-color: gray; color: white; text-align:center;">DISCONTINUED</td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mt-3">
                                                        <div class="col-12">
                                                            <div class="table-responsive grid-container">
                                                                <asp:GridView runat="server" ID="gvListCellular" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListCellular_RowDataBound">
                                                                    <RowStyle />
                                                                    <Columns>
                                                                        <asp:BoundField DataField="FabricType" HeaderText="" />
                                                                        <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col7" HeaderText="Colour" />
                                                                        <asp:BoundField DataField="Col8" HeaderText="Colour" />
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="tab-pane fade" id="list-fabricchart" role="tabpanel" aria-labelledby="listFabricChart">
                                    <div class="row mt-5" runat="server" id="divErrorFabricChart">
                                        <div class="col-12">
                                            <div class="alert alert-danger">
                                                <span runat="server" id="msgErrorFabricChart"></span>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mt-5" runat="server">
                                        <div class="col-12 col-sm-12 col-lg-7">
                                            <asp:Panel runat="server" DefaultButton="btnFabricChart" Width="100%">
                                                <div class="input-group">
                                                    <span class="input-group-text">Fabric Type : </span>
                                                    <asp:TextBox runat="server" ID="txtSearchFabricChart" CssClass="form-control" placeholoder="" autocomplete="off"></asp:TextBox>
                                                    <asp:Button runat="server" ID="btnFabricChart" CssClass="btn btn-primary" Text="Search" OnClick="btnFabricChart_Click" />
                                                </div>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                    <div class="row mt-3">
                                        <div class="col-12">
                                            <div class="table-responsive grid-container">
                                                <asp:GridView runat="server" ID="gvListFabricChart" CssClass="table table-bordered table-hover" AutoGenerateColumns="false" ShowFooter="true" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvListFabricChart_RowDataBound">
                                                    <RowStyle />
                                                    <Columns>
                                                        <asp:BoundField DataField="FabricName" HeaderText="Fabric Type" />
                                                        <asp:BoundField DataField="Roller" HeaderText="Roller" />
                                                        <asp:BoundField DataField="RomanClassic" HeaderText="Roman (Classic)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        <asp:BoundField DataField="RomanPlantation" HeaderText="Roman (Plantation)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        <asp:BoundField DataField="RomanSewless" HeaderText="Roman (Sewless)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        <asp:BoundField DataField="PGPlain" HeaderText="PG (Plain)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        <asp:BoundField DataField="PGPlantation" HeaderText="PG (Plantation)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        <asp:BoundField DataField="PGSewless" HeaderText="PG (Sewless)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        <asp:BoundField DataField="Vertical89mm" HeaderText="Vertical (89mm)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
                                                        <asp:BoundField DataField="Vertical127mm" HeaderText="Vertical (127mm)" HeaderStyle-Wrap="true" FooterStyle-Wrap="true" />
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
                </div>
                <footer class="mt-4 py-3 border-top">
                    <div class="container">
                        <div class="d-flex justify-content-between align-items-center text-muted small">
                            <div>&copy; 2026 <strong>BOOS</strong>. All rights reserved.</div>
                            <div>
                                <a runat="server" href="~/contact" class="text-muted me-3 text-decoration-none">Contact</a>
                                <a runat="server" href="~/about" class="text-muted me-3 text-decoration-none">About</a>
                                <a runat="server" href="~/faq" class="text-muted text-decoration-none">Fax</a>
                            </div>
                        </div>
                    </div>
                </footer>
            </div>
        </div>

        <asp:HiddenField ID="selected_tab" runat="server" />

        <script src="/Assets/vendors/perfect-scrollbar/perfect-scrollbar.min.js"></script>
        <script src="/Assets/js/bootstrap.bundle.min.js"></script>
        <script src="/Assets/vendors/sweetalert2/sweetalert2.all.min.js"></script>
        <script src="/Assets/vendors/choices.js/choices.min.js"></script>
        <script src="/Assets/js/pages/form-element-select.js"></script>
        <script src="/Assets/js/pages/horizontal-layout.js"></script>

        <script type="text/javascript">
            function activateCurrentTab() {
                var tabId = $("#<%= selected_tab.ClientID %>").val();
                if (!tabId) tabId = "list-roller";
                $('#dvTab a[href="#' + tabId + '"]').tab('show');
            }

            function pageInit() {
                $(document).off("click.stocktab").on("click.stocktab", "#dvTab a", function () {
                    var tabId = $(this).attr("href").replace("#", "");
                    $("#<%= selected_tab.ClientID %>").val(tabId);
                });
                activateCurrentTab();
            }

            $(document).ready(function () {
                pageInit();
            });

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                pageInit();
            });

            window.history.replaceState(null, null, window.location.href);
        </script>
    </form>
</body>
</html>
