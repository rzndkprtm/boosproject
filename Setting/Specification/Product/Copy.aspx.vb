Imports System.Data
Imports System.Data.SqlClient

Partial Class Setting_Specification_Product_Copy
    Inherits Page

    Dim settingClass As New SettingClass
    Dim myConn As String = ConfigurationManager.ConnectionStrings("DefaultConnection").ConnectionString
    Dim dataLog As Object() = Nothing

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pageAccess As Boolean = LoginAccess("Load")
        If pageAccess = False Then
            Response.Redirect("~/setting/specification/product", False)
            Exit Sub
        End If

        If Not IsPostBack Then
            MessageError(False, String.Empty)

            BindDesignType()
            BindBlindType(ddlDesignType.SelectedValue)
            BindTubeType()
            BindControlType()
            BindColourType()
            BindCompanyDetail()
        End If
    End Sub

    Protected Sub ddlDesignType_SelectedIndexChanged(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        BindBlindType(ddlDesignType.SelectedValue)
    End Sub

    Protected Sub btnSubmit_Click(sender As Object, e As EventArgs)
        MessageError(False, String.Empty)
        Try
            If ddlDesignType.SelectedValue = "" Then
                MessageError(True, "DESIGN TYPE IS REQUIRED !")
                Exit Sub
            End If

            If msgError.InnerText = "" Then
                Dim blindString As String = String.Empty
                Dim tubeString As String = String.Empty
                Dim controlString As String = String.Empty
                Dim colourString As String = String.Empty

                If Not ddlBlindType.SelectedValue = "" Then blindString = "AND BlindId='" & ddlBlindType.SelectedValue & "'"

                If Not ddlTubeType.SelectedValue = "" Then tubeString = "AND TubeType='" & ddlTubeType.SelectedValue & "'"

                If Not ddlControlType.SelectedValue = "" Then controlString = "AND ControlType='" & ddlControlType.SelectedValue & "'"

                If Not ddlColourType.SelectedValue = "" Then colourString = "AND ColourType='" & ddlColourType.SelectedValue & "'"

                Dim stringSplit As String = String.Empty
                Dim subCompany As String = String.Empty
                If Not String.IsNullOrEmpty(ddlCompanyDetail.SelectedValue) Then
                    stringSplit = "CROSS APPLY STRING_SPLIT(CompanyDetailId, ',') AS CompanyDetailArray"
                    subCompany = "CompanyDetailArray.VALUE='" & ddlCompanyDetail.SelectedValue & "'"
                End If

                Dim thisString As String = String.Format("SELECT Id FROM Products {0} WHERE DesignId={1} {2} {3} {4} {5} {6} ORDER BY Id ASC", stringSplit, ddlDesignType.SelectedValue, blindString, tubeString, controlString, colourString, subCompany)

                Dim thisData As DataTable = settingClass.GetDataTable(thisString)
                If Not thisData.Rows.Count = 0 Then
                    For i As Integer = 0 To thisData.Rows.Count - 1
                        Dim productId As String = thisData.Rows(i)("Id").ToString()

                        Dim newId As String = settingClass.CreateId("SELECT TOP 1 Id FROM Products ORDER BY Id DESC")
                        Using thisConn As New SqlConnection(myConn)
                            Using thisCmd As SqlCommand = New SqlCommand("INSERT INTO Products SELECT @NewId, DesignId, BlindId, CompanyDetailId, JobSheetId, Name + ' - Copy', InvoiceName, TubeType, ControlType, ColourType, Description, NULL FROM Products WHERE Id=@Id", thisConn)
                                thisCmd.Parameters.AddWithValue("@NewId", newId)
                                thisCmd.Parameters.AddWithValue("@Id", productId)
                                thisConn.Open()
                                thisCmd.ExecuteNonQuery()
                            End Using
                        End Using

                        dataLog = {"Products", productId, Session("LoginId").ToString(), "Product Created | Duplicated of " & productId}
                        settingClass.Logs(dataLog)
                    Next
                End If

                Response.Redirect("~/setting/specification/product", False)
            End If
        Catch ex As Exception
            MessageError(True, ex.ToString())
            If Not Session("RoleName") = "Developer" Then
                MessageError(True, "PLEASE CONTACT IT SUPPORT AT REZA@BIGBLINDS.CO.ID !")
            End If
        End Try
    End Sub

    Protected Sub btnCancel_Click(sender As Object, e As EventArgs)
        Response.Redirect("~/setting/specification/product", False)
    End Sub

    Protected Sub BindDesignType()
        ddlDesignType.Items.Clear()
        Try
            ddlDesignType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Designs ORDER BY Name ASC")
            ddlDesignType.DataTextField = "Name"
            ddlDesignType.DataValueField = "Id"
            ddlDesignType.DataBind()

            If ddlDesignType.Items.Count > 1 Then
                ddlDesignType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlDesignType.Items.Clear()
        End Try
    End Sub

    Protected Sub BindBlindType(designId As String)
        ddlBlindType.Items.Clear()
        Try
            If Not String.IsNullOrEmpty(designId) Then
                ddlBlindType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM Blinds WHERE DesignId='" & designId & "' ORDER BY Name ASC")
                ddlBlindType.DataTextField = "Name"
                ddlBlindType.DataValueField = "Id"
                ddlBlindType.DataBind()

                If ddlBlindType.Items.Count > 1 Then
                    ddlBlindType.Items.Insert(0, New ListItem("", ""))
                End If
            End If
        Catch ex As Exception
            ddlBlindType.Items.Clear()
        End Try
    End Sub

    Protected Sub BindTubeType()
        ddlTubeType.Items.Clear()
        Try
            ddlTubeType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM ProductTubes ORDER BY Name ASC")
            ddlTubeType.DataTextField = "Name"
            ddlTubeType.DataValueField = "Id"
            ddlTubeType.DataBind()

            If ddlTubeType.Items.Count > 1 Then
                ddlTubeType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlTubeType.Items.Clear()
        End Try
    End Sub

    Protected Sub BindControlType()
        ddlControlType.Items.Clear()
        Try
            ddlControlType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM ProductControls ORDER BY Name ASC")
            ddlControlType.DataTextField = "Name"
            ddlControlType.DataValueField = "Id"
            ddlControlType.DataBind()

            If ddlControlType.Items.Count > 1 Then
                ddlControlType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlControlType.Items.Clear()
        End Try
    End Sub

    Protected Sub BindColourType()
        ddlColourType.Items.Clear()
        Try
            ddlColourType.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM ProductColours ORDER BY Name ASC")
            ddlColourType.DataTextField = "Name"
            ddlColourType.DataValueField = "Id"
            ddlColourType.DataBind()

            If ddlColourType.Items.Count > 1 Then
                ddlColourType.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlColourType.Items.Clear()
        End Try
    End Sub

    Protected Sub BindCompanyDetail()
        ddlCompanyDetail.Items.Clear()
        Try
            ddlCompanyDetail.DataSource = settingClass.GetDataTable("SELECT Id, Name FROM CompanyDetails ORDER BY Name ASC")
            ddlCompanyDetail.DataTextField = "Name"
            ddlCompanyDetail.DataValueField = "Id"
            ddlCompanyDetail.DataBind()

            If ddlCompanyDetail.Items.Count > 1 Then
                ddlCompanyDetail.Items.Insert(0, New ListItem("", ""))
            End If
        Catch ex As Exception
            ddlCompanyDetail.Items.Clear()
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
