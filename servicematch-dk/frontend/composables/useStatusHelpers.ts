import type { ServiceRequestStatus, OfferStatus } from '~/types/api.types'

export function useStatusHelpers() {
  function requestStatusLabel(s: ServiceRequestStatus): string {
    const map: Record<ServiceRequestStatus, string> = {
      Open: 'Åben', OfferReceived: 'Tilbud modtaget', Accepted: 'Accepteret',
      Declined: 'Afvist', Cancelled: 'Annulleret', Completed: 'Afsluttet',
    }
    return map[s]
  }

  function requestStatusBadge(s: ServiceRequestStatus): string {
    const map: Record<ServiceRequestStatus, string> = {
      Open: 'badge-blue', OfferReceived: 'badge-yellow', Accepted: 'badge-green',
      Declined: 'badge-red', Cancelled: 'badge-gray', Completed: 'badge-purple',
    }
    return map[s]
  }

  function offerStatusLabel(s: OfferStatus): string {
    const map: Record<OfferStatus, string> = {
      Pending: 'Afventer', Accepted: 'Accepteret', Declined: 'Afvist', Countered: 'Modtilbud',
    }
    return map[s]
  }

  function offerStatusBadge(s: OfferStatus): string {
    const map: Record<OfferStatus, string> = {
      Pending: 'badge-yellow', Accepted: 'badge-green', Declined: 'badge-red', Countered: 'badge-blue',
    }
    return map[s]
  }

  return { requestStatusLabel, requestStatusBadge, offerStatusLabel, offerStatusBadge }
}
