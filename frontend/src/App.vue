<template>
  <v-app>
    <v-app-bar 
      color="primary" 
      density="comfortable"
    >
      <v-app-bar-title>Invoice Approval</v-app-bar-title>
    </v-app-bar>

    <v-main class="bg-grey-lighten-4">
      <v-container 
        class="py-8" 
        style="max-width: 560px"
      >
        <InvoiceForm @result="onResult" />

        <v-expand-transition>
          <ApprovalWorkflow
            v-if="approvers !== null"
            :approvers="approvers"
            class="mt-6"
          />
        </v-expand-transition>
      </v-container>
    </v-main>
  </v-app>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import InvoiceForm from './components/InvoiceForm.vue'
import ApprovalWorkflow from './components/ApprovalWorkflow.vue'

const approvers = ref<string[] | null>(null)

function onResult(result: string[]) {
  approvers.value = result
}
</script>
