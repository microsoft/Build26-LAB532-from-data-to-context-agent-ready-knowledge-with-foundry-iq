<#
.SYNOPSIS
    Seeds sample emails into the lab user's mailbox via Microsoft Graph API.
.DESCRIPTION
    Uses the service principal's app-only token to create emails directly in
    the lab user's inbox. Requires Mail.ReadWrite application permission
    (admin-consented) on the service principal.
.PARAMETER UserUpn
    The lab user's UPN (email) to seed messages into.
.PARAMETER TenantId
    The Entra tenant ID for token acquisition.
.PARAMETER ClientId
    The service principal's app/client ID.
.PARAMETER ClientSecret
    The service principal's client secret.
#>
param(
    [Parameter(Mandatory)][string]$UserUpn,
    [Parameter(Mandatory)][string]$TenantId,
    [Parameter(Mandatory)][string]$ClientId,
    [Parameter(Mandatory)][string]$ClientSecret
)

$ErrorActionPreference = "Stop"

function Log {
    param([string]$msg)
    Write-Host "[$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')] $msg"
}

# Get an app-only token for Microsoft Graph
function Get-GraphToken {
    $body = @{
        grant_type    = "client_credentials"
        client_id     = $ClientId
        client_secret = $ClientSecret
        scope         = "https://graph.microsoft.com/.default"
    }
    $response = Invoke-RestMethod -Method POST `
        -Uri "https://login.microsoftonline.com/$TenantId/oauth2/v2.0/token" `
        -ContentType "application/x-www-form-urlencoded" `
        -Body $body
    return $response.access_token
}

# Create a message directly in the user's Inbox folder
function Create-MailMessage {
    param(
        [string]$Token,
        [string]$UserId,
        [hashtable]$Message
    )
    $headers = @{
        Authorization  = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    $url = "https://graph.microsoft.com/v1.0/users/$UserId/mailFolders/inbox/messages"
    $body = $Message | ConvertTo-Json -Depth 10
    $result = Invoke-RestMethod -Method POST -Uri $url -Headers $headers -Body $body
    return $result
}

# Move a message to Inbox
function Move-ToInbox {
    param(
        [string]$Token,
        [string]$UserId,
        [string]$MessageId
    )
    $headers = @{
        Authorization  = "Bearer $Token"
        "Content-Type" = "application/json"
    }
    $url = "https://graph.microsoft.com/v1.0/users/$UserId/messages/$MessageId/move"
    $body = @{ destinationId = "inbox" } | ConvertTo-Json
    Invoke-RestMethod -Method POST -Uri $url -Headers $headers -Body $body | Out-Null
}

# ============================================================
# Email definitions
# ============================================================

$emails = @(
    @{
        subject          = "Urgent: Professional Claw Hammer out of stock at Seattle store"
        sender           = @{
            emailAddress = @{
                name    = "Marcus Chen"
                address = "marcus.chen@zavadiy.com"
            }
        }
        toRecipients     = @(
            @{
                emailAddress = @{
                    name    = $UserUpn
                    address = $UserUpn
                }
            }
        )
        body             = @{
            contentType = "HTML"
            content     = @"
<p>Hey,</p>
<p>The store manager at <strong>Zava Retail Seattle</strong> says customers keep asking for the <strong>Professional Claw Hammer 16oz</strong> (SKU: HTHM001600) but the shelf has been empty for three days now. We've had at least six customer complaints this week alone.</p>
<p>Can you check stock levels across our other stores and see if we can do a transfer? Seattle is our highest-traffic location and this is one of our best sellers — we can't afford to be out of stock heading into summer.</p>
<p>If other stores are low too, we may need to escalate to procurement for an emergency reorder.</p>
<p>Thanks,<br/>Marcus Chen<br/>Regional Operations Manager</p>
"@
        }
        receivedDateTime = (Get-Date).AddHours(-4).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        isRead           = $false
        importance       = "high"
    },
    @{
        subject          = "RE: Weekly inventory report - Seattle flagged"
        sender           = @{
            emailAddress = @{
                name    = "Priya Sharma"
                address = "priya.sharma@zavadiy.com"
            }
        }
        toRecipients     = @(
            @{
                emailAddress = @{
                    name    = $UserUpn
                    address = $UserUpn
                }
            }
        )
        body             = @{
            contentType = "HTML"
            content     = @"
<p>Hi,</p>
<p>Just following up on Marcus's note — I pulled the weekly inventory report and Seattle is showing <strong>zero stock</strong> on several hand tools, not just the claw hammer. The Professional Claw Hammer (HTHM001600) is the most requested one though.</p>
<p>I checked Bellevue and Redmond and they seem to have some units. Could you verify the exact numbers in the system and coordinate a store-to-store transfer if the quantities allow?</p>
<p>Also worth checking if Tacoma or Online have surplus — their seasonal demand is usually lower this time of year.</p>
<p>Let me know if you need help with the transfer paperwork.</p>
<p>Thanks,<br/>Priya Sharma<br/>Inventory Analyst</p>
"@
        }
        receivedDateTime = (Get-Date).AddHours(-2).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        isRead           = $false
        importance       = "normal"
    },
    @{
        subject          = "Customer escalation - hammer unavailable again"
        sender           = @{
            emailAddress = @{
                name    = "Jordan Lee"
                address = "jordan.lee@zavadiy.com"
            }
        }
        toRecipients     = @(
            @{
                emailAddress = @{
                    name    = $UserUpn
                    address = $UserUpn
                }
            }
        )
        body             = @{
            contentType = "HTML"
            content     = @"
<p>Hi team,</p>
<p>Got another customer complaint on the support line — a contractor needed 5 units of the Professional Claw Hammer 16oz for a job this weekend and was told Seattle is completely out. He's threatening to switch to Home Depot if we can't fulfill by Friday.</p>
<p>This is the third escalation this week on the same SKU (HTHM001600). Can someone please check what's available across all stores and get a transfer or restock in motion ASAP?</p>
<p>Thanks,<br/>Jordan Lee<br/>Customer Support Lead</p>
"@
        }
        receivedDateTime = (Get-Date).AddHours(-1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        isRead           = $false
        importance       = "high"
    }
)

# ============================================================
# Main
# ============================================================

Log "Seeding $($emails.Count) emails into mailbox: $UserUpn"

$token = Get-GraphToken
Log "Acquired Graph API token"

foreach ($email in $emails) {
    $msg = Create-MailMessage -Token $token -UserId $UserUpn -Message $email
    Log "Created: $($email.subject)"
}

Log "Email seeding complete!"
