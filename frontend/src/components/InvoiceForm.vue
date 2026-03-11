<template>
  <v-card rounded="lg">
    <v-card-title class="pa-5 pb-1">
      Invoice Details
    </v-card-title>
    <v-card-text class="pa-5 pt-3">
      <v-form
        ref="formRef"
        v-model="isFormValid"
        @submit.prevent="handleSubmit"
      >
        <v-text-field
          v-model.number="amount"
          label="Amount"
          prefix="$"
          type="number"
          variant="outlined"
          :rules="amountRules"
          min="0"
          max="999999999999.99"
          step="0.01"
          class="mb-2"
        />

        <v-checkbox
          v-model="isPreferredVendor"
          label="Preferred Vendor"
          hint="Skips Manager-level approval"
          persistent-hint
          color="primary"
          density="comfortable"
          class="mb-4"
        />

        <v-btn
          type="submit"
          color="primary"
          block
          :loading="loading"
          :disabled="!isFormValid"
        >
          Determine Approvers
        </v-btn>
      </v-form>

      <v-alert
        v-if="error"
        type="error"
        variant="tonal"
        closable
        class="mt-4"
        @click:close="error = ''"
      >
        {{ error }}
      </v-alert>
    </v-card-text>
  </v-card>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { determineApprovers } from '../api/invoiceApi'

const emit = defineEmits<{
  result: [approvers: string[]]
}>()

const amount = ref<number | null>(null)
const isPreferredVendor = ref(false)
const isFormValid = ref(false)
const loading = ref(false)
const error = ref('')

const MAX_AMOUNT = 999_999_999_999.99

const amountRules = [
  (v: number | null) => v !== null && v !== undefined || 'Amount is required',
  (v: number | null) => (v !== null && v > 0) || 'Amount must be greater than zero',
  (v: number | null) => (v !== null && v <= MAX_AMOUNT) || 'Invoice amount exceeds the maximum allowed value',
]

async function handleSubmit() {
  if (!isFormValid.value || amount.value === null) return

  loading.value = true
  error.value = ''

  try {
    const response = await determineApprovers({
      amount: amount.value,
      isPreferredVendor: isPreferredVendor.value,
    })
    emit('result', response.approvers)
  } catch (err: unknown) {
    const message =
      err instanceof Error ? err.message : 'Failed to determine approvers'
    error.value = message
  } finally {
    loading.value = false
  }
}
</script>
