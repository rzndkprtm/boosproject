Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Blind_Edit
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/blind", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("blindid")) Then
            Response.Redirect("~/setting/specification/blind", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("blindid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "BLIND TYPE IS REQUIRED !")
                Exit Sub
            End If
            If ddlDesign.Text = "" Then
                MessageError(True, "DESIGN NAME IS REQUIRED !")
                Exit Sub
            End If
            If lbCompany.SelectedValue = "" Then
                MessageError(True, "COMPANY IS REQUIRED !")
                Exit Sub
            End If
            If msgError.InnerText = "" Then
                Dim companyDetail As String = String.Empty
                Dim aliasName As String = txtAlias.Text.Trim()

                If Not lbCompany.SelectedValue = "" Then
                    companyDetail = String.Join(",", lbCompany.Items.Cast(Of ListItem)().Where(Function(i) i.Selected).Select(Function(i) i.Value))
                End If
                If String.IsNullOrEmpty(txtAlias.Text) Then
                    aliasName = txtName.Text.Trim()
                End If

                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using thisCmd As SqlCommand = New SqlCommand("UPDATE Blinds SET DesignId=@DesignId, CompanyDetailId=@CompanyDetailId, ItemCode=@ItemCode, Name=@Name, Alias=@Alias, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        thisCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        thisCmd.Parameters.AddWithValue("@DesignId", ddlDesign.SelectedValue)
                        thisCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                        thisCmd.Parameters.AddWithValue("@ItemCode", txtItemCode.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        thisCmd.Parameters.AddWithValue("@Alias", aliasName)
                        thisCmd.Parameters.AddWithValue("@Description", descText)
                        thisCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)
                        thisConn.Open()
                        thisCmd.ExecuteNonQuery()
                    End Using
                End Using

                dataLog = {"Blinds", lblId.Text, Session("LoginId").ToString(), "Blind Type Updated"}
                settingClass.Logs(dataLog)

                Response.Redirect("~/setting/specification/blind", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/blind", False)
    End Sub

    Protected Sub BindData(blindId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Blinds WHERE Id='" & blindId & "'")
            If thisData Is Nothing Then Exit Sub

            BindDesign()
            BindCompany()

            ddlDesign.SelectedValue = thisData("DesignId").ToString()
            txtName.Text = thisData("Name").ToString()
            txtItemCode.Text = thisData("ItemCode").ToString()
            txtAlias.Text = thisData("Alias").ToString()
            txtDescription.Text = thisData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(thisData("Active"))

            If Not thisData("CompanyDetailId").ToString() = "" Then
                Dim companyArray() As String = thisData("CompanyDetailId").ToString().Split(",")
                For Each i In companyArray
                    If Not (i.Equals(String.Empty)) Then
                        lbCompany.Items.FindByValue(i).Selected = True
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


    Protected Sub BindDesign()
        ddlDesign.Items.Clear()
        Try
            ddlDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs ORDER BY Name ASC")
            ddlDesign.DataTextField = "Name"
            ddlDesign.DataValueField = "Id"
            ddlDesign.DataBind()

            If ddlDesign.Items.Count > 0 Then
                ddlDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompany()
        lbCompany.Items.Clear()
        Try
            lbCompany.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails WHERE Active=1 ORDER BY Name ASC")
            lbCompany.DataTextField = "Name"
            lbCompany.DataValueField = "Id"
            lbCompany.DataBind()

            If lbCompany.Items.Count > 0 Then
                lbCompany.Items.Insert(0, New ListItem("", ""))
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
