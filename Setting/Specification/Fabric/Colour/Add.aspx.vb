
Imports OfficeOpenXml.LoadFunctions

Partial Class Setting_Specification_Fabric_Colour_Add
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        'Dim pageAccess As Boolean = LoginAccess("Load")
        'If pageAccess = False Then
        '    Response.Redirect("~/setting/specification/fabric/colour", False)
        '    Exit Sub
        'End If

        If Not String.IsNullOrEmpty(Request.QueryString("fabricid")) Then
            lblFabricId.Text = Request.QueryString("fabricid").ToString()
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindFabric(lblFabricId.Text)
            BindCompanyDetail(ddlFabricType.SelectedValue)
        End If
    End Sub

    Protected Sub ddlFabricType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindCompanyDetail(ddlFabricType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try

        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/specification/fabric/colour"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/specification/fabric/detail?customerid={0}", ddlFabricType.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindFabric(fabricId As String)
        Try
            Dim query As String = "SELECT Id, Name FROM Fabrics"

            If Not String.IsNullOrEmpty(fabricId) Then
                query &= " WHERE Id='" & fabricId & "'"
            End If

            ddlFabricType.DataSource = settingClass.GetDataTable(query)
            ddlFabricType.DataTextField = "Name"
            ddlFabricType.DataValueField = "Id"
            ddlFabricType.DataBind()

            If ddlFabricType.Items.Count > 1 Then
                ddlFabricType.Items.Insert(0, New ListItem("", ""))
            End If

        Catch ex As Exception
            ddlFabricType.Items.Clear()

            If Session("RoleName") = "Developer" Then
                MessageError(True, ex.ToString())
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail(fabricId As String)
        lbCompanyDetail.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(fabricId) Then
                Dim companyDetailId As String = settingClass.GetItemData("SELECT CompanyDetailId FROM Fabrics WHERE Id='" & fabricId & "'")
                lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM CompanyDetails WHERE Id IN (SELECT value FROM STRING_SPLIT('" & companyDetailId & "', ','))")
                lbCompanyDetail.DataTextField = "Name"
                lbCompanyDetail.DataValueField = "Id"
                lbCompanyDetail.DataBind()

                For Each item As ListItem In lbCompanyDetail.Items
                    item.Selected = True
                Next

                If lbCompanyDetail.Items.Count > 0 Then
                    lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub MessageError(visible As Boolean, message As String)
        divError.Visible = visible : msgError.InnerText = message
    End Sub

    Protected Function LoginAccess(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim accessClass As New AccessClass

            Return accessClass.GetLoginAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
