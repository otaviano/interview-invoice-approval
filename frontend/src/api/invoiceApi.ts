import axios from 'axios'

export interface DetermineApproversRequest {
  amount: number
  isPreferredVendor: boolean
}

export interface DetermineApproversResponse {
  approvers: string[]
}

const apiClient = axios.create({
  baseURL: '/api',
  headers: { 'Content-Type': 'application/json' },
})

export async function determineApprovers(
  request: DetermineApproversRequest
): Promise<DetermineApproversResponse> {
  const { data } = await apiClient.post<DetermineApproversResponse>(
    '/invoices/determine-approvers',
    request
  )
  return data
}
