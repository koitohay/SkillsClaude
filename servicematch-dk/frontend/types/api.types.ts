export interface AuthTokenDto {
  token: string
  role: 'Client' | 'Provider'
  userId: string
}

export interface ClientDto {
  id: string
  fullName: string
  email: string
  phoneNumber: string
}

export interface ServiceProviderDto {
  id: string
  companyName: string
  contactName: string
  email: string
  phoneNumber: string
  address: string
  city: string
  cvrNumber: string
  isVerified: boolean
  categoryIds: number[]
  services: ProviderServiceListingDto[]
}

export interface NewServiceDto {
  categoryId: number | null
  name: string
  description: string
  basePrice: number
}

export interface ProviderServiceListingDto {
  id: string
  name: string
  description: string
  basePrice: number
  categoryId: number | null
}

export interface ProviderWithServicesDto {
  id: string
  companyName: string
  contactName: string
  city: string
  phoneNumber: string
  services: ProviderServiceListingDto[]
}

export interface CategoryDto {
  id: number
  name: string
  slug: string
}

export type ServiceRequestStatus = 'Open' | 'OfferReceived' | 'Accepted' | 'Declined' | 'Cancelled' | 'Completed'
export type OfferStatus = 'Pending' | 'Accepted' | 'Declined' | 'Countered'
export type NegotiationStatus = 'Pending' | 'Accepted' | 'Declined'
export type NegotiationInitiator = 'Client' | 'Provider'

export interface ServiceRequestDto {
  id: string
  clientId: string
  categoryId: number | null
  categoryName: string | null
  freeTextDescription: string | null
  requestedDate: string
  requestedTime: string
  city: string
  status: ServiceRequestStatus
  createdAt: string
  offers?: OfferDto[]
}

export interface NegotiationDto {
  id: string
  initiatedBy: NegotiationInitiator
  proposedPrice: number
  message: string | null
  status: NegotiationStatus
  createdAt: string
}

export interface OfferDto {
  id: string
  serviceRequestId: string
  serviceProviderId: string
  price: number
  message: string | null
  status: OfferStatus
  createdAt: string
  negotiations: NegotiationDto[]
}

export interface ChatMessageDto { role: 'user' | 'assistant'; content: string }
export interface ChatReplyDto { reply: string }

export interface FeaturedOfferDto {
  offerId: string
  serviceRequestId: string
  categoryName: string
  city: string
  price: number
  providerMessage: string | null
  curationReason: string
  offerCreatedAt: string
}
