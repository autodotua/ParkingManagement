<template>
  <div>
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>我的资金</span>
        <el-button
          style="float: right; padding: 3px 0"
          type="text"
          @click="jump('TransactionRecord')"
        >交易记录</el-button>
      </div>
      <el-row>
        <el-col :span="12">余额：</el-col>
        <el-col :span="8">{{displayBalance}}</el-col>
        <el-col :span="4">
          <el-button size="mini" @click="jump('Recharge')">充值</el-button>
        </el-col>
      </el-row>
      <br />
      <el-row>
        <el-col :span="8">月租到期：</el-col>
        <el-col :span="12">{{displayExpireTime}}</el-col>
        <el-col :span="4">
          <el-button size="mini">续期</el-button>
        </el-col>
      </el-row>
    </el-card>
    <br />
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>我的车辆</span>
        <el-button style="float: right; padding: 3px 0" type="text" @click="jump('Car')">管理</el-button>
      </div>
      <el-table :data="cars" style="width: 100%">
        <el-table-column prop="licensePlate" label="车牌" width="120"></el-table-column>
        <el-table-column prop="records" label="停车次数" width="80"></el-table-column>
      </el-table>
    </el-card>
    <br />
    <el-card class="box-card">
      <div slot="header" class="clearfix">
        <span>停车场状态</span>
        <!-- <el-button style="float: right; padding: 3px 0" type="text" @click="jump('/Car')">管理</el-button> -->
      </div>
      <div v-for="park in parks" :key="park.id">
        <h4>{{park.name}}</h4>
        <a :href="getImageUrl(park.id)" style="width:100%">
          <img :src="getImageUrl(park.id)" style="width:100%" />
        </a>
      </div>
    </el-card>
  </div>
</template>
<script lang="ts">
import Vue from "vue";
import Cookies from "js-cookie";
import { withToken, getUrl, showError, jump, formatDateTime } from "../common";
export default Vue.extend({
  name: "Home",
  data() {
    return {
      cars: [],
      balance: 0,
      expireTime: "",
      parks: []
    };
  },
  computed: {
    displayBalance(): string {
      return this.balance + "元";
    },
    displayExpireTime(): string {
      if (this.expireTime.startsWith("0001")) {
        return "无";
      }
      return formatDateTime(this.expireTime, false);
    }
  },
  methods: {
    jump: jump,
    getImageUrl(id: number): string {
      return getUrl("Home", "ParkImage") + "/" + id;
    }
  },
  components: {},
  mounted: function() {
    this.$nextTick(function() {
      if (Cookies.get("userID") == undefined) {
        return;
      }
      Vue.axios
        .post(getUrl("Home", "Index"), withToken({}))
        .then(response => {
          this.cars = response.data.data.cars;
          this.balance = response.data.data.balance;
          this.expireTime = response.data.data.expireTime;
          this.parks = response.data.data.parks;
          console.log(this.parks);
        })
        .catch(showError);
    });
  }
});
</script>
