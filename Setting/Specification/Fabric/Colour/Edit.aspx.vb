Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Fabric_Colour_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/fabric/colour", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("fabriccolourid")) Then
            Response.Redirect("~/setting/specification/fabric/colour", False)
            Exit Sub
        End If

        If Not String.IsNullOrEmpty(Request.QueryString("returnpage")) Then
            lblReturnPage.Text = Request.QueryString("returnpage").ToString()
        End If

        lblId.Text = Request.QueryString("fabriccolourid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub ddlFabricType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindCompanyDetail(ddlFabricType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlFabricType.SelectedValue = "" Then
                MessageError(True, "FABRIC TYPE IS REQUIRED !")
                Exit Sub
            End If
            If lbCompanyDetail.SelectedValue = "" Then
                MessageError(True, "SUB COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If ddlFactory.SelectedValue = "" Then
                MessageError(True, "FACTORY IS REQUIRED !")
                Exit Sub
            End If
            If txtColour.Text = "" Then
                MessageError(True, "COLOUR IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim fabricName As String = settingClass.GetItemData("SELECT Name FROM Fabrics WHERE Id='" & ddlFabricType.SelectedValue & "'")
                Dim fabricColourName As String = String.Format("{0} {1}", fabricName, txtColour.Text.Trim())

                Dim companyDetailId As String = String.Join(",", lbCompanyDetail.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE FabricColours SET CompanyDetailId=@CompanyDetailId, BoeId=@BoeId, InventoryId=@InventoryId, Factory=@Factory, Name=@Name, Colour=@Colour, Width=@Width, RollQty=@RollQty, EtaFactory=@EtaFactory, Description=@Description WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@FabricId", ddlFabricType.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetailId)
                        thisCmd.Parameters.AddWithValue("@BoeId", If(String.IsNullOrEmpty(txtBoeId.Text), CType(DBNull.Value, Object), txtBoeId.Text))
                        thisCmd.Parameters.AddWithValue("@InventoryId", If(String.IsNullOrEmpty(txtInventoryId.Text), CType(DBNull.Value, Object), txtInventoryId.Text))
                        thisCmd.Parameters.AddWithValue("@Factory", ddlFactory.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@Name", fabricColourName)
                        thisCmd.Parameters.AddWithValue("@Colour", txtColour.Text)
                        thisCmd.Parameters.AddWithValue("@Width", txtWidth.Text)
                        thisCmd.Parameters.AddWithValue("@RollQty", If(String.IsNullOrEmpty(txtRollQty.Text), CType(DBNull.Value, Object), txtRollQty.Text))
                        thisCmd.Parameters.AddWithValue("@EtaFactory", If(String.IsNullOrEmpty(txtETAFactory.Text), CType(DBNull.Value, Object), txtETAFactory.Text))
                        thisCmd.Parameters.AddWithValue("@Description", txtDescription.Text)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"FabricColours", lblId.Text, Session("LoginId").ToString(), "Fabric Colour Updated"}
                settingClass.Logs(dataLog)

                url = "~/setting/specification/fabric/colour"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", ddlFabricType.SelectedValue)
                End If
                Response.Redirect(url, False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        url = "~/setting/specification/fabric/colour"
        If lblReturnPage.Text = "detail" Then
            url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", ddlFabricType.SelectedValue)
        End If
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(fabricColourId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM FabricColours WHERE Id='" & fabricColourId & "'")
            If thisData Is Nothing Then
                url = "~/setting/specification/fabric/colour"
                If lblReturnPage.Text = "detail" Then
                    url = String.Format("~/setting/specification/fabric/detail?fabricid={0}", ddlFabricType.SelectedValue)
                End If
                Response.Redirect(url, False)
                Exit Sub
            End If

            Dim fabricId As String = thisData("FabricId").ToString()

            BindFabric()
            BindCompanyDetail(fabricId)

            ddlFabricType.SelectedValue = thisData("FabricId").ToString()
            If lblReturnPage.Text = "detail" Then
                ddlFabricType.Enabled = False
            End If
            txtBoeId.Text = thisData("BoeId").ToString()
            txtInventoryId.Text = thisData("InventoryId").ToString()
            ddlFactory.SelectedValue = thisData("Factory").ToString()
            txtColour.Text = thisData("Colour").ToString()
            txtWidth.Text = thisData("Width").ToString()
            txtRollQty.Text = thisData("RollQty").ToString()
            txtETAFactory.Text = thisData("EtaFactory").ToString()
            txtDescription.Text = thisData("Description").ToString()

            If Not String.IsNullOrWhiteSpace(thisData("CompanyDetailId").ToString()) Then
                For Each i As String In thisData("CompanyDetailId").ToString().Split(","c)
                    Dim value As String = i.Trim()

                    If value <> "" Then
                        Dim item As ListItem = lbCompanyDetail.Items.FindByValue(value)
                        If item IsNot Nothing Then
                            item.Selected = True
                        End If
                    End If
                Next
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindFabric()
        Try
            ddlFabricType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Fabrics")
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
