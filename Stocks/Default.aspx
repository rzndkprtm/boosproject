<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Default.aspx.vb" Inherits="Stocks_Default" MasterPageFile="~/Site.Master" MaintainScrollPositionOnPostback="true" Debug="true" Title="Stocks" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .grid-container { width:100%; height:calc(100vh - 150px); overflow:auto; border:1px solid #ddd; }
        .grid-container table { width:100%; border-collapse:collapse; table-layout:auto; }
        .grid-container td, .grid-container th { white-space:nowrap; padding:6px 10px; }
        .grid-container th { position:sticky; top:0; background:#f5f5f5; z-index:3; }
        .grid-container td:first-child, .grid-container th:first-child { position:sticky; left:0; background:#fff; z-index:2; white-space:normal; word-break:break-word; min-width:220px; max-width:320px; }
        .grid-container th:first-child { z-index:5; background:#f5f5f5; }
        .stock-toolbar { display:flex; justify-content:space-between; align-items:center; gap:12px; flex-wrap:wrap; padding:15px; margin-bottom:20px; background:#f8f9fa; border:1px solid #e9ecef; border-radius:10px; }
        .stock-toolbar .btn { border-radius:7px; padding:9px 16px; font-weight:500; font-size:13px; }
        .stock-toolbar .btn-generate { background:#435ebe; color:#fff; border:1px solid #435ebe; }
        .stock-toolbar .btn-generate:hover { background:#364b98; border-color:#364b98; color:#fff; }
        .stock-toolbar .dropdown-menu { border:1px solid #e9ecef; border-radius:8px; padding:6px; box-shadow:0 5px 20px rgba(0,0,0,.08); }
        .stock-toolbar .dropdown-item { padding:9px 12px; border-radius:5px; font-size:13px; }
        .stock-toolbar .dropdown-item:hover { background:#f0f2f5; }
        .stock-grid { width:100%; min-width:1100px; border-collapse:separate !important; border-spacing:0; margin-bottom:0; }
        .stock-grid th { background:#f1f3f8 !important; color:#344054; text-align:center; vertical-align:middle; font-weight:600; padding:12px; }
        .stock-grid td { text-align:center; vertical-align:middle; min-width:150px; height:75px; padding:10px; white-space:normal !important; }
        .stock-grid td:first-child { min-width:220px; max-width:280px; text-align:left; font-weight:600; background:#f8f9fc; }
        .stock-colour { font-weight:700; font-size:16px; }
        .stock-info { font-size:15px; margin-top:5px; line-height:1.5; }
        .stock-empty { background:#f8f9fa; color:#adb5bd; }
        .stock-grid-container { max-height:600px; overflow-y:auto; overflow-x:auto; position:relative; }
        .stock-grid thead th { position:sticky; top:0; z-index:10; background:#f8f9fa !important; box-shadow:0 2px 4px rgba(0,0,0,.08); }
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
                    <div class="card-body pb-0">
                        <div class="stock-toolbar" runat="server" id="divAction">
                            <div>
                                <div class="fw-bold">Stock Management</div>
                                <small class="text-muted">Stock information</small>
                            </div>
                            <div class="d-flex gap-2 flex-wrap">
                                <div class="dropdown">
                                    <button class="btn btn-outline-secondary dropdown-toggle" type="button" data-bs-toggle="dropdown" aria-expanded="false">
                                        <i class="bi bi-grid"></i> Stock Category
                                    </button>
                                    <ul class="dropdown-menu dropdown-menu-end">
                                        <li><h6 class="dropdown-header">Stock Categories</h6></li>
                                        <li><a runat="server" class="dropdown-item" href="~/stocks/jpmd" target="_blank"><i class="bi bi-building me-2"></i>JPMD</a></li>
                                        <li><a runat="server" class="dropdown-item" href="~/stocks/cws" target="_blank"><i class="bi bi-building me-2"></i>CWS</a></li>
                                        <li><a runat="server" class="dropdown-item" href="~/stocks/accent" target="_blank"><i class="bi bi-building me-2"></i>Accent</a></li>
                                        <li><a runat="server" class="dropdown-item" href="~/stocks/global" target="_blank"><i class="bi bi-building me-2"></i>Sunlight Global</a></li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <asp:UpdatePanel ID="updateData" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="card-body">
                                <div class="list-group list-group-horizontal-sm mb-1 text-center" id="dvTab" role="tablist">
                                    <a class="list-group-item list-group-item-action active" id="listRoller" data-bs-toggle="list" href="#list-roller" role="tab">Roller</a>
                                    <a class="list-group-item list-group-item-action" id="listVertical" data-bs-toggle="list" href="#list-vertical" role="tab">Vertical</a>
                                    <a class="list-group-item list-group-item-action" id="listSaphora" data-bs-toggle="list" href="#list-saphora" role="tab">Saphora</a>
                                    <a class="list-group-item list-group-item-action" id="listCellular" data-bs-toggle="list" href="#list-cellular" role="tab">Cellular Shades</a>
                                    <a class="list-group-item list-group-item-action" id="listDesignShades" data-bs-toggle="list" href="#list-designshades" role="tab">Design Shades</a>
                                    <a class="list-group-item list-group-item-action" id="listCurtain" data-bs-toggle="list" href="#list-curtain" role="tab">Curtain</a>
                                    <% If Convert.ToString(Session("CompanyId")) <> "2" Then %>
                                    <a class="list-group-item list-group-item-action" id="listOutdoor" data-bs-toggle="list" href="#list-outdoor" role="tab">Outdoor</a>
                                    <% End If %>
                                    <a class="list-group-item list-group-item-action" id="listVenetian" data-bs-toggle="list" href="#list-venetian" role="tab">Venetian Aluminium</a>
                                    <a class="list-group-item list-group-item-action" id="listFabricChart" data-bs-toggle="list" href="#list-fabricchart" role="tab">Fabric Chart</a>
                                </div>
                                <div class="tab-content text-justify">
                                    <div class="tab-pane fade show active" id="list-roller" role="tabpanel" aria-labelledby="listRoller">
                                        <div class="row mt-5" runat="server" id="divErrorRoller">
                                            <div class="col-12">
                                                <div class="alert alert-danger"><span runat="server" id="msgErrorRoller"></span></div>
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
                                        <div class="row mt-5">
                                            <div class="col-12 col-sm-12 col-lg-7">
                                                <asp:Panel runat="server" DefaultButton="btnSearchRoller" Width="100%">
                                                    <div class="input-group">
                                                        <span class="input-group-text">Fabric Type : </span>
                                                        <asp:TextBox runat="server" ID="txtSearchRoller" CssClass="form-control" autocomplete="off"></asp:TextBox>
                                                        <asp:Button runat="server" ID="btnSearchRoller" CssClass="btn btn-primary" Text="Search" OnClick="btnSearchRoller_Click" />
                                                    </div>
                                                </asp:Panel>
                                            </div>
                                        </div>
                                        <div class="row mt-3">
                                            <div class="col-12">
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvRoller" CssClass="table table-bordered table-hover stock-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvRoller_RowDataBound" OnPreRender="gvRoller_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="Name" HeaderText="Fabric Type" />
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
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvVertical" CssClass="table table-bordered table-hover stock-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvVertical_RowDataBound" OnPreRender="gvVertical_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="Name" HeaderText="Fabric Name" />
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
                                    <div class="tab-pane fade" id="list-saphora" role="tabpanel" aria-labelledby="listSaphora">
                                        <div class="row mt-5" runat="server" id="divErrorSaphora">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorSaphora"></span>
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
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvSaphora" CssClass="table table-bordered table-hover stock-grid" AutoGenerateColumns="false" ShowHeaderWhenEmpty="true" EmptyDataText="DATA NOT FOUND :)" EmptyDataRowStyle-HorizontalAlign="Center" OnRowDataBound="gvSaphora_RowDataBound" OnPreRender="gvSaphora_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="Name" HeaderText="Fabric Name" />
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
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvCellular" CssClass="table table-bordered table-hover stock-grid" AutoGenerateColumns="false" ShowHeader="true" OnRowDataBound="gvCellular_RowDataBound" OnPreRender="gvCellular_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="Name" HeaderText="Fabric Name" />
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
                                    <div class="tab-pane fade" id="list-designshades" role="tabpanel" aria-labelledby="listDesignShades">
                                        <div class="row mt-5" runat="server" id="divErrorDesignShades">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorDesignShades"></span>
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
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvDesignShades" CssClass="table table-bordered table-hover stock-grid" AutoGenerateColumns="false" ShowHeader="true" OnRowDataBound="gvDesignShades_RowDataBound" OnPreRender="gvDesignShades_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="Name" HeaderText="Fabric Name" />
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
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvCurtain" AutoGenerateColumns="False" CssClass="table table-bordered table-hover stock-grid" GridLines="None" ShowHeader="True" OnRowDataBound="gvCurtain_RowDataBound" OnPreRender="gvCurtain_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="Name" HeaderText="Fabric Type">
                                                                <ItemStyle CssClass="fw-bold text-nowrap" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="Col1" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col2" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col3" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col4" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col5" HeaderText="Colour" />
                                                            <asp:BoundField DataField="Col6" HeaderText="Colour" />
                                                        </Columns>
                                                        <EmptyDataTemplate>
                                                            <div class="text-center text-muted py-4">No fabric stock found.</div>
                                                        </EmptyDataTemplate>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                        
                                    </div>
                                    <div class="tab-pane fade" id="list-outdoor" role="tabpanel" aria-labelledby="listOutdoor">
                                        <div class="row mt-5" runat="server" id="divErrorOutdoor">
                                            <div class="col-12">
                                                <div class="alert alert-danger">
                                                    <span runat="server" id="msgErrorOutdoor"></span>
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
                                        <div class="row mt-5">
                                            <div class="col-12">
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvOutdoor" AutoGenerateColumns="False" CssClass="table table-bordered table-hover stock-grid" GridLines="None" ShowHeader="True" OnRowDataBound="gvOutdoor_RowDataBound" OnPreRender="gvOutdoor_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="Name" HeaderText="Fabric Type">
                                                                <ItemStyle CssClass="fw-bold text-nowrap" />
                                                            </asp:BoundField>
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
                                                <div class="stock-grid-container">
                                                    <asp:GridView runat="server" ID="gvListVenetian" CssClass="table table-bordered table-hover stock-grid" AutoGenerateColumns="false" ShowHeader="true" OnRowDataBound="gvListVenetian_RowDataBound" OnPreRender="gvListVenetian_PreRender">
                                                        <Columns>
                                                            <asp:BoundField DataField="BlindName" HeaderText="Type" />
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
                            <div class="card-footer" runat="server" id="divCompanyDetail">
                                <div class="row">
                                    <div class="col-12 col-sm-12 col-lg-4 d-flex justify-content-end">
                                        <div class="input-group">
                                            <span class="input-group-text">Company Detail</span>
                                            <asp:DropDownList runat="server" ID="ddlCompanyDetail" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlCompanyDetail_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>                    
                </div>
            </div>
        </section>
    </div>
    <div id="loadingOverlay" style="display:none; position:fixed; top:0; left:0; width:100%; height:100%; background:rgba(255,255,255,.5); z-index:99999;">
        <div class="position-absolute top-50 start-50 translate-middle">
            <div class="card shadow">
                <div class="card-body text-center">
                    <div class="spinner-border"></div>
                    <div class="mt-2">Loading...</div>
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
    <asp:HiddenField ID="selected_tab" runat="server" />
    <script type="text/javascript">
        function showLoading() {
            $("#loadingOverlay").show();
        }
        function hideLoading() {
            $("#loadingOverlay").hide();
        }
        function activateCurrentTab() {
            var tabId = $("#<%= selected_tab.ClientID %>").val();
            if (!tabId) {
                tabId = "list-roller";
            }
            var $tab = $('#dvTab a[href="#' + tabId + '"]');
            if ($tab.length) {
                $tab.tab('show');
            }
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
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_beginRequest(function () {
                showLoading();
            });
            prm.add_endRequest(function () {
                hideLoading();
                pageInit();
            });
        });
        function showWaiting(hideModal = null) {
            $("#modalWaiting").modal("show");
            setTimeout(function () {
                $("#modalWaiting").modal("hide");
                if (hideModal) {
                    $(`#${hideModal}`).modal("hide");
                }
            }, 5000);

            return true;
        }
        ["modalWaiting"].forEach(function (id) {
            document.getElementById(id).addEventListener("hide.bs.modal", function () {
                document.activeElement.blur();
                document.body.focus();
            });
        });
        window.history.replaceState(null, null, window.location.href);
    </script>
</asp:Content>
