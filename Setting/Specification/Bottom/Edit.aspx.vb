Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Bottom_Edit
    Inherits Page

    Dim settingClass As New SettingClass

    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim url As String = String.Empty

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = PageAction("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/bottom", False)
            Exit Sub
        End If

        If String.IsNullOrEmpty(Request.QueryString("bottomid")) Then
            Response.Redirect("~/setting/specification/bottom", False)
            Exit Sub
        End If

        lblId.Text = Request.QueryString("bottomid").ToString()
        If Not IsPostBack Then
            MessageError(False, String.Empty)
            BindData(lblId.Text)
        End If
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If txtName.Text = "" Then
                MessageError(True, "BOTTOM NAME IS REQUIRED !")
                Exit Sub
            End If

            Dim designData As String = String.Empty
            For Each item As ListItem In lbDesign.Items
                If item.Selected Then
                    If Not String.IsNullOrEmpty(item.Selected) Then
                        designData += item.Value & ","
                    End If
                End If
            Next
            If designData = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            Dim cdData As String = String.Empty
            For Each item As ListItem In lbCompanyDetail.Items
                If item.Selected Then
                    If Not String.IsNullOrEmpty(item.Selected) Then
                        cdData += item.Value & ","
                    End If
                End If
            Next
            If cdData = "" Then
                MessageError(True, "COMPANY DETAIL IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim designType As String = designData.Remove(designData.Length - 1).ToString()
                Dim companyDetail As String = cdData.Remove(cdData.Length - 1).ToString()
                Dim descText As String = txtDescription.Text.Replace(vbCrLf, "").Replace(vbCr, "").Replace(vbLf, "")

                Using thisConn As New SqlConnection(myConn)
                    Using myCmd As SqlCommand = New SqlCommand("UPDATE Bottoms SET Name=@Name, DesignId=@DesignId, CompanyDetailId=@CompanyDetailId, Description=@Description, Active=@Active WHERE Id=@Id", thisConn)
                        myCmd.Parameters.AddWithValue("@Id", lblId.Text)
                        myCmd.Parameters.AddWithValue("@Name", txtName.Text.Trim())
                        myCmd.Parameters.AddWithValue("@DesignId", designType)
                        myCmd.Parameters.AddWithValue("@CompanyDetailId", companyDetail)
                        myCmd.Parameters.AddWithValue("@Description", descText)
                        myCmd.Parameters.AddWithValue("@Active", ddlActive.SelectedValue)

                        thisConn.Open()
                        myCmd.ExecuteNonQuery()
                    End Using
                End Using

                Dim dataLog As Object() = {"Bottoms", lblId.Text, Session("LoginId").ToString(), "Bottom Type Created"}
                settingClass.Logs(dataLog)

                url = String.Format("~/setting/specification/bottom/detail?bottomid={0}", lblId.Text)
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
        url = String.Format("~/setting/specification/bottom/detail?bottomid={0}", lblId.Text)
        Response.Redirect(url, False)
    End Sub

    Protected Sub BindData(bottomId As String)
        Try
            Dim thisData As DataRow = settingClass.GetDataRow("SELECT * FROM Bottoms WHERE Id='" & bottomId & "'")
            If thisData Is Nothing Then
                Response.Redirect("~/setting/specification/bottom", False)
                Exit Sub
            End If

            BindDesign()
            BindCompanyDetail()

            txtName.Text = thisData("Name").ToString()
            txtDescription.Text = thisData("Description").ToString()
            ddlActive.SelectedValue = Convert.ToInt32(thisData("Active"))

            If Not thisData("DesignId").ToString() = "" Then
                Dim designArray() As String = thisData("DesignId").ToString().Split(",")
                For Each i In designArray
                    If Not (i.Equals(String.Empty)) Then
                        lbDesign.Items.FindByValue(i).Selected = True
                    End If
                Next
            End If

            If Not thisData("CompanyDetailId").ToString() = "" Then
                Dim companyArray() As String = thisData("CompanyDetailId").ToString().Split(",")

                For Each i In companyArray
                    If Not (i.Equals(String.Empty)) Then
                        lbCompanyDetail.Items.FindByValue(i).Selected = True
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
        lbDesign.Items.Clear()
        Try
            lbDesign.DataSource = settingClass.GetDataTable("SELECT * FROM Designs CROSS APPLY STRING_SPLIT(AppliesTo, ',') AS thisArray WHERE thisArray.VALUE='Bottoms' AND Active=1 ORDER BY Name ASC")
            lbDesign.DataTextField = "Name"
            lbDesign.DataValueField = "Id"
            lbDesign.DataBind()

            If lbDesign.Items.Count > 0 Then
                lbDesign.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        lbCompanyDetail.Items.Clear()
        Try
            lbCompanyDetail.DataSource = settingClass.GetDataTable("SELECT * FROM CompanyDetails WHERE Active=1 ORDER BY Name ASC")
            lbCompanyDetail.DataTextField = "Name"
            lbCompanyDetail.DataValueField = "Id"
            lbCompanyDetail.DataBind()

            If lbCompanyDetail.Items.Count > 0 Then
                lbCompanyDetail.Items.Insert(0, New ListItem("", ""))
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

    Protected Function PageAction(action As String) As Boolean
        Try
            Dim roleId As String = Session("RoleId").ToString()
            Dim levelId As String = Session("LevelId").ToString()
            Dim actionClass As New ActionClass

            Return actionClass.GetActionAccess(roleId, levelId, Page.Title, action)
        Catch ex As Exception
            Response.Redirect("~/account/login", False)
            HttpContext.Current.ApplicationInstance.CompleteRequest()
            Return False
        End Try
    End Function
End Class
