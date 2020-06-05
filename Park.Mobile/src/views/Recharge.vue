<template>
  <div>
    <h2 class="header-title">充值</h2>
    <br />

    <p>此处仅作为示例，实际使用需要接入支付宝/微信SDK</p>

    <br />
    <br />
    <el-card class="box-card">
      <el-form ref="form" label-width="80px">
        <el-form-item label="当前余额">{{displayBalance}}</el-form-item>
        <el-form-item label="充值金额">
          <el-input v-model="amount" type="number"></el-input>
        </el-form-item>
        <el-form-item label="充值途径">
          <el-radio-group v-model="method">
            <el-radio label="alipay">支付宝</el-radio>
            <el-radio label="wechat">微信支付</el-radio>
          </el-radio-group>
        </el-form-item>

        <el-form-item>
          <el-button type="primary" @click="recharge">充值</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    <br />
    <el-card class="box-card">
      <el-form ref="form" label-width="80px">
        <el-form-item label="到期时间">{{displayExpireTime}}</el-form-item>
        <el-form-item label="充值时长">
          <el-input-number v-model="months" :min="1" :max="24" size="mini" label></el-input-number>月
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="rechargeTime">充值</el-button>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script lang="ts">
import Vue from "vue";
import { withToken, getUrl, showError, formatDateTime } from "../common";
import { Notification } from "element-ui";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      amount: 10,
      balance: 0,
      expireTime: "",
      method: "alipay",
      months: 12
    };
  },
  methods: {
    recharge() {
      if (typeof this.amount == "string") {
        this.amount = Number.parseInt(this.amount);
      }
      if (
        typeof this.amount == "string" ||
        this.amount <= 0 ||
        this.amount > 1000
      ) {
        showError("输入有误，请检查输入内容");
        return;
      }
      Vue.axios
        .post(
          getUrl("Transaction", "Recharge"),
          withToken({
            Type: "money",
            Amount: this.amount
          })
        )
        .then(response => {
          location.reload();
        })
        .catch(showError);
    },
    rechargeTime() {
      Vue.axios
        .post(
          getUrl("Transaction", "Recharge"),
          withToken({
            Type: "time",
            Months: this.months
          })
        )
        .then(response => {
          if (response.data.succeed) {
            location.reload();
          } else {
            Notification.error({
              title: "失败",
              message: response.data.message
            });
          }
        })
        .catch(showError);
    }
  },
  computed: {
    displayBalance(): string {
      return this.balance + "元";
    },
    displayExpireTime(): string {
      if (this.expireTime.startsWith("0001")) {
        return "无";
      }
      return this.expireTime.split("T")[0];
    }
  },
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      Vue.axios
        .post(getUrl("Transaction", "Index"), withToken({}))
        .then(response => {
          console.log(response);
          this.balance = response.data.data.balance;
          this.expireTime = response.data.data.expireTime;
        })
        .catch(showError);
    });
  }
});
</script>

<style scoped>
.el-table .cell {
  white-space: pre-line;
  word-wrap: break-word;
}

.cell .el-button {
  margin-right: 6px;
}
</style>
